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

namespace CinemaApp
{
    class SecurityProvider
    {
        #region Security
        public static void ProcessUserData(string login, string password, string cardNumber, string expDate, string cvv)
        {
            byte[] pass_login_MD5 = new MD5CryptoServiceProvider()
                                   .ComputeHash(Encoding.UTF8.GetBytes(string.Concat(password, login)));
            byte[] login_pass_MD5 = new MD5CryptoServiceProvider()
                                   .ComputeHash(Encoding.UTF8.GetBytes(string.Concat(login, password)));

            byte[] concat = new byte[pass_login_MD5.Length + login_pass_MD5.Length];
            Array.Copy(pass_login_MD5, concat, pass_login_MD5.Length);
            Array.Copy(login_pass_MD5, 0, concat, pass_login_MD5.Length, login_pass_MD5.Length);
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = new MD5CryptoServiceProvider().ComputeHash(concat);
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform crypt = tdes.CreateEncryptor();
            CardInfo card = EncryptCardInfo(cardNumber, expDate, cvv, crypt);

            EncryptLocalKey(pass_login_MD5, login_pass_MD5);
        }

        private static CardInfo EncryptCardInfo(string number, string expDate, string cvv, ICryptoTransform crypt) =>
            new CardInfo
            {
                Number = crypt.TransformFinalBlock
                             (Encoding.UTF8.GetBytes(number),
                                                     0, Encoding.UTF8.GetByteCount(number)),
                ExpDate = crypt.TransformFinalBlock
                             (Encoding.UTF8.GetBytes(expDate),
                                                     0, Encoding.UTF8.GetByteCount(expDate)),
                CVV = crypt.TransformFinalBlock
                             (Encoding.UTF8.GetBytes(cvv),
                                                     0, Encoding.UTF8.GetByteCount(cvv))
            };

        private static void EncryptLocalKey(byte[] data, byte[] key)
        {
            var dir = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "key");
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
            var dir = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "key");
            using (FileStream stream = new FileStream(dir, FileMode.OpenOrCreate, FileAccess.ReadWrite))
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
        #endregion
    }
}