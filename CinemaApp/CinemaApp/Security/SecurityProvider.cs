using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Security.Cryptography;
using System.IO;
using CinemaApp.Model;
using CinemaApp.Exceptions;

namespace CinemaApp.Security
{
    class SecurityProvider
    {
        #region Security
        public static void ProcessUserData(string login, string password, string cardNumber, string expDate, string cvv)
        {
            byte[] localKey = new MD5CryptoServiceProvider()
                                   .ComputeHash(Encoding.UTF8.GetBytes(string.Concat(password, login)));
            byte[] serverKey = new MD5CryptoServiceProvider()
                                   .ComputeHash(Encoding.UTF8.GetBytes(string.Concat(login, password)));

            byte[] concat = new byte[localKey.Length + serverKey.Length];
            Array.Copy(localKey, concat, localKey.Length);
            Array.Copy(serverKey, 0, concat, localKey.Length, serverKey.Length);
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = new MD5CryptoServiceProvider().ComputeHash(concat);
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform crypt = tdes.CreateEncryptor();
            CardInfo card = EncryptCardInfo(cardNumber, expDate, cvv, crypt);

            EncryptLocalKey(localKey, serverKey);        
            var result = Server.ServerRequest.SignUpNewUser(login, BitConverter.ToString(serverKey).Replace("-", string.Empty), card);
            if (result) CreateUserTokenFile(login, new MD5CryptoServiceProvider().ComputeHash(serverKey));
            else throw new RegistrationFailedException();
        }

        private static CardInfo EncryptCardInfo(string number, string expDate, string cvv, ICryptoTransform crypt) =>
            new CardInfo
            {
                Number = BitConverter.ToString(crypt.TransformFinalBlock
                             (Encoding.UTF8.GetBytes(number),
                                                     0, Encoding.UTF8.GetByteCount(number))).Replace("-", string.Empty),
                ExpDate = BitConverter.ToString(crypt.TransformFinalBlock
                             (Encoding.UTF8.GetBytes(expDate),
                                                     0, Encoding.UTF8.GetByteCount(expDate))).Replace("-", string.Empty),
                CVV = BitConverter.ToString(crypt.TransformFinalBlock
                             (Encoding.UTF8.GetBytes(cvv),
                                                     0, Encoding.UTF8.GetByteCount(cvv))).Replace("-", string.Empty)
            };

        private static void EncryptLocalKey(byte[] data, byte[] key)
        {
            var dir = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.Path, "key.dat");
            using (FileStream stream = new FileStream(dir, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            using (TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider())
            {
                stream.SetLength(0);
                tdes.Key = key;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;
                byte[] buffer = tdes.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
                stream.Write(buffer, 0, buffer.Length);
                stream.Close();
            }
        }

        private static byte[] DecryptLocalKey(byte[] key)
        {
            var dir = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.Path, "key.dat");
            using (FileStream stream = new FileStream(dir, FileMode.Open, FileAccess.Read))
            using (TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider())
            {
                tdes.Key = key;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;
                byte[] buffer = new byte[24];
                stream.Read(buffer, 0, buffer.Length);
                byte[] data = tdes.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length);
                stream.Close();
                return data;
            }
        }

        private static void CreateUserTokenFile(string login, byte[] keyHash)
        {
            var dir = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.Path, "token.dat");
            using (FileStream stream = new FileStream(dir, FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"{login}:{BitConverter.ToString(keyHash).Replace("-", string.Empty)}");
        }

        public static bool VerifyUser(string pass)
        {
            string[] buffer;
            var dir = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.Path, "token.dat");
            using (FileStream stream = new FileStream(dir, FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(stream))
                buffer = reader.ReadLine().Split(':');
            if (buffer[1] == BitConverter.ToString(
                                new MD5CryptoServiceProvider()
                                .ComputeHash(new MD5CryptoServiceProvider()
                                .ComputeHash(Encoding.UTF8.GetBytes(string.Concat(buffer[0], pass))))).Replace("-", string.Empty))
                return true;
            else return false;

        }
        public static bool BookSeats(Session session, IEnumerable<int> seatIdOnScheme)
        {
            byte[] keyBuffer;
            string token;

            var dir = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.Path, "key.dat");
            using (FileStream stream = new FileStream(dir, FileMode.Open, FileAccess.Read))
            {
                keyBuffer = new byte[stream.Length];
                stream.Read(keyBuffer, 0, keyBuffer.Length);
            }

            dir = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.Path, "token.dat");
            using (FileStream stream = new FileStream(dir, FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(stream))
                token = reader.ReadLine().Split(':')[1];

            string appKey = BitConverter.ToString(keyBuffer).Replace("-", string.Empty);

            bool result = Server.ServerRequest.BookSeats(token, appKey, session.ID.ToString(), session.Hall, seatIdOnScheme);
            if (result)
            {
                foreach (int seatId in seatIdOnScheme)
                    session.BookedSeats.Add(new Seat() { ID = ((session.Hall - 1) * 80) + seatId,
                                                         Hall = session.Hall,
                                                         Row = seatId / 10 + 1,
                                                         Number = seatId % 10 + 1
                                                       });
                return true;
            }
            else throw new BookingFailedException();
        }

        public static bool ProcessLogin(string email, string password)
        {
            byte[] userToken = new MD5CryptoServiceProvider().ComputeHash(
                               new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(string.Concat(email, password))));
            var result = Server.ServerRequest.LogIn(email, BitConverter.ToString(userToken));
            if (result) CreateUserTokenFile(email, userToken);
            return result;
        }
        #endregion

        public static byte[] StringToByteArray(string hex) =>
            Enumerable.Range(0, hex.Length)
                      .Where(x => x % 2 == 0)
                      .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                      .ToArray();
    }
}