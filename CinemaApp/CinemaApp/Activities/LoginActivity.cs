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
using System.IO;

namespace CinemaApp.Activities
{
    [Activity(Label = "CinemaApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class LoginActivity : Activity
    {
        private EditText _email;
        private EditText _password;
        private Button _signUp;
        private Button _logIn;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (File.Exists(Path.Combine(Android.OS.Environment.ExternalStorageDirectory.Path, "token.dat")))
                StartActivity(new Intent(this, typeof(MoviesLoadingPageActivity)));
            else
            {
                SetContentView(Resource.Layout.LoginPageLayout);
                FindViews();
                Initialize();
            }
        }

        private void FindViews()
        {
            _email = FindViewById<EditText>(Resource.Id.loginPage_email);
            _password = FindViewById<EditText>(Resource.Id.loginPage_password);
            _signUp = FindViewById<Button>(Resource.Id.loginPage_buttonSignUp);
            _logIn = FindViewById<Button>(Resource.Id.loginPage_buttonLogIn);
        }

        private void Initialize()
        {
            _signUp.Click += (object sender, EventArgs e) => StartActivity(new Intent(this, typeof(RegistrationActivity)));
            _logIn.Click += (object sender, EventArgs e) => ProcessLogin();
        }

        private void ProcessLogin()
        {
            bool result = Security.SecurityProvider.ProcessLogin(_email.Text, _password.Text);
            if (result) StartActivity(new Intent(this, typeof(MoviesLoadingPageActivity)));
            else Toast.MakeText(this, "Login failed!", ToastLength.Long).Show();
        }
    }
}