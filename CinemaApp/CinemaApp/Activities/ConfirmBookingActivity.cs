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
using CinemaApp.Model;
using Newtonsoft.Json;
using CinemaApp.Resources.views;
using Android.Hardware.Fingerprints;
using CinemaApp.Security;

namespace CinemaApp.Activities
{
    [Activity(Label = "ConfirmBookingActivity")]
    public class ConfirmBookingActivity : Activity
    {
        
        private GridLayout _seatsScheme;
        private EditText _password;
        private Button _buttonSubmit;

        private Movie _movie;
        private Day _showDay;
        private Session _session;
        private IEnumerable<int> _seats;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ConfirmBookingPageLayout);
            FindViews();
            Initialize();
            Authenticate();
        }

        private void FindViews()
        {
            _seatsScheme = FindViewById<GridLayout>(Resource.Id.seats);
            _password = FindViewById<EditText>(Resource.Id.confirmBooking_password);
            _buttonSubmit = FindViewById<Button>(Resource.Id.confirmBooking_buttonSubmit);
        }

        private void Initialize()
        {
            _movie = Schedule.Movies.Find(m => m.ID == Intent.GetIntExtra("MovieID", 0));
            _showDay = _movie.ShowDays.Find(d => d.Sessions.Exists( s => s.ID == Intent.GetIntExtra("SessionID", 0)));
            _session = _showDay.Sessions.Find(s => s.ID == Intent.GetIntExtra("SessionID", 0));
            _seats = JsonConvert.DeserializeObject<IEnumerable<int>>(Intent.GetStringExtra("SelectedSeats"));
        }

        private void Authenticate()
        {
            FingerprintManager fingerprint = this.GetSystemService(FingerprintService) as FingerprintManager;
            KeyguardManager keyGuard = GetSystemService(KeyguardService) as KeyguardManager;
            Android.Content.PM.Permission permission = CheckSelfPermission(Android.Manifest.Permission.UseFingerprint);
            if (fingerprint.IsHardwareDetected
                && keyGuard.IsKeyguardSecure
                && fingerprint.HasEnrolledFingerprints
                && permission == Android.Content.PM.Permission.Granted)
            {
                const int flags = 0;
                CryptoObjectFactory cryptoHelper = new CryptoObjectFactory();
                CancellationSignal cancellationSignal = new CancellationSignal();
                FingerprintManager.AuthenticationCallback authCallback = new AuthCallback(this);
                fingerprint.Authenticate(cryptoHelper.BuildCryptoObject(), cancellationSignal, flags, authCallback, null);
            }
        }

        public void OnAuthenticationSucceeded()
        {
            Toast.MakeText(this, "Fingerprint scan succeeded", ToastLength.Long).Show();
            //some action here
        }

        public void OnAuthenticationFailed()
        {
            Toast.MakeText(this, "Fingerprint scan failed", ToastLength.Long).Show();
            Authenticate();
        }
    }
}