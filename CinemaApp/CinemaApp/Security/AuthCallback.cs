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
using Android.Hardware.Fingerprints;
using Javax.Crypto;
using Android.Util;
using CinemaApp.Activities;

namespace CinemaApp.Security
{
    class AuthCallback : FingerprintManager.AuthenticationCallback
    {
        private static readonly byte[] SECRET_BYTES = { 1, 4, 8, 8, 3, 5, 9, 4, 2 };
        private static readonly string TAG = "CinemaApp";
        private readonly ConfirmBookingActivity _currentActivity;

        public AuthCallback() { }

        public AuthCallback(ConfirmBookingActivity activity)
        {
            _currentActivity = activity;
        }

        public override void OnAuthenticationSucceeded(FingerprintManager.AuthenticationResult result)
        {
            if (result.CryptoObject.Cipher != null)
            {
                try
                {
                    byte[] doFinalResult = result.CryptoObject.Cipher.DoFinal(SECRET_BYTES);
                    _currentActivity.OnAuthenticationSucceeded();
                }
                catch (BadPaddingException bpe)
                {
                    _currentActivity.OnAuthenticationFailed();
                    Log.Error(TAG, "Failed to encrypt the data with the generated key." + bpe);
                }
                catch (IllegalBlockSizeException ibse)
                {
                    _currentActivity.OnAuthenticationFailed();
                    Log.Error(TAG, "Failed to encrypt the data with the generated key." + ibse);
                }
            }
            else
            {
                _currentActivity.OnAuthenticationSucceeded();
            }
        }

        public override void OnAuthenticationFailed()
        {
            _currentActivity.OnAuthenticationFailed();
        }
    }
}