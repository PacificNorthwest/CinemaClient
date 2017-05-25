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
using CinemaApp.Server;
using CinemaApp.Model;
using System.IO;
using CinemaApp;
using Android;

namespace CinemaApp.Activities
{
    [Activity(Label = "CinemaApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class RegistrationActivity : Activity
    {
        private EditText _login;
        private EditText _password;
        private EditText _confirmPassword;
        private EditText _cardNumber;
        private EditText _expDate;
        private EditText _cvv;
        private Button _signUpButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.RegistrationPage);
            InitializeViews();
        }

        private void InitializeViews()
        {
            _login = FindViewById<EditText>(Resource.Id.login);
            _password = FindViewById<EditText>(Resource.Id.password);
            _confirmPassword = FindViewById<EditText>(Resource.Id.confirmPassword);
            _cardNumber = FindViewById<EditText>(Resource.Id.card);
            _expDate = FindViewById<EditText>(Resource.Id.expDate);
            _cvv = FindViewById<EditText>(Resource.Id.cvv);
            _signUpButton = FindViewById<Button>(Resource.Id.buttonSignUp);

            _signUpButton.Click += SignUpButton_Click;
            _expDate.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
                                    { if ((sender as EditText).Length() == 2) (sender as EditText).Text += "/";
                                        (sender as EditText).SetSelection((sender as EditText).Length()); };
        }

        private void SignUpButton_Click(object sender, EventArgs e)
        {
            if (_password.Text == _confirmPassword.Text)
            {
                try
                {
                    SecurityProvider.ProcessUserData(_login.Text,
                                                     _password.Text,
                                                     _cardNumber.Text,
                                                     _expDate.Text,
                                                     _cvv.Text);
                    Toast.MakeText(this, "Registration succesful", ToastLength.Long).Show();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                }
            }
            else
                Toast.MakeText(this, "Not matching passwords!", ToastLength.Short).Show();
        }
    }
}