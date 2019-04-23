using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace CinemaServer.Security
{
    public class SecurityProvider
    {
        public static bool ProcessPayment(string encryptedAppKey, string serverKey, string encryptedCardNumber, string encryptedExpDate, string encryptedCVV, List<int> prices)
        {
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = Hex.StringToByteArray(serverKey);
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            byte[] encryptedAppKeyBytes = Hex.StringToByteArray(encryptedAppKey);
            byte[] appKeyBytes = tdes.CreateDecryptor().TransformFinalBlock(encryptedAppKeyBytes, 0, encryptedAppKeyBytes.Length);
            byte[] serverKeyBytes = Hex.StringToByteArray(serverKey);

            byte[] concat = new byte[appKeyBytes.Length + serverKeyBytes.Length];
            Array.Copy(appKeyBytes, concat, appKeyBytes.Length);
            Array.Copy(serverKeyBytes, 0, concat, appKeyBytes.Length, serverKeyBytes.Length);
            tdes.Key = new MD5CryptoServiceProvider().ComputeHash(concat);
            ICryptoTransform crypt = tdes.CreateDecryptor();

            byte[] encryptedCardNumberBytes = Hex.StringToByteArray(encryptedCardNumber);
            byte[] encryptedExpDateBytes = Hex.StringToByteArray(encryptedExpDate);
            byte[] encryptedCVVBytes = Hex.StringToByteArray(encryptedCVV);

            string decryptedCardNumber = Encoding.UTF8.GetString(
                                                  crypt.TransformFinalBlock(
                                                      encryptedCardNumberBytes, 0, encryptedCardNumberBytes.Length));
            string decryptedExpDate = Encoding.UTF8.GetString(
                                               crypt.TransformFinalBlock(
                                                   encryptedExpDateBytes, 0, encryptedExpDateBytes.Length));
            string decryptedCVV = Encoding.UTF8.GetString(
                                           crypt.TransformFinalBlock(
                                               encryptedCVVBytes, 0, encryptedCVVBytes.Length));

            bool result = MakePayment(decryptedCardNumber, decryptedExpDate, decryptedCVV, prices);
            return result;
        }

        private static bool MakePayment(string cardNumber, string expDate, string cvv, List<int> prices) => true; //For testing purposes
        
    }
}