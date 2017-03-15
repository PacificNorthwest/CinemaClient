using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using CinemaApp.Model;
using CinemaApp.Server;
using System;

namespace CinemaApp.Activities
{
    [Activity(Label = "CinemaApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Button _connectButton;
        private TextView _textView;
        private List<Movie> _movies;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView (Resource.Layout.Main);
            Initialize();
        }

        private void Initialize()
        {
            _connectButton = FindViewById<Button>(Resource.Id.connectButton);
            _textView = FindViewById<TextView>(Resource.Id.textView);
            _connectButton.Click += ConnectButton_Click;
        }

        private void ConnectButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                _movies = ServerRequest.LoadMovieList();
                PrintData();
            }
            catch(Exception ex)
            {
                _textView.Text = ex.ToString();
            }
        }

        private void PrintData()
        {
            _textView.Text = $"Title: {_movies[0].Title}\nDirector: {_movies[0].Director}\n3D: {_movies[0].Is3D}\nDate: {_movies[0].Date.ToString("dd MM yyyy")}\nTime: {_movies[0].Time.ToString("hh:mm")}";
        }
    }
}

