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

namespace CinemaApp
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
            if (result) CreateUserToken(login, serverKey);
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

        private static void CreateUserToken(string login, byte[] baseHash)
        {
            var dir = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.Path, "token.dat");
            using (FileStream stream = new FileStream(dir, FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"{login}:{BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(baseHash)).Replace("-", string.Empty)}");
        }
        #endregion
    }
}