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
        private List<Movie> _movies;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView (Resource.Layout.Main);
            Initialize();
            Populate();
        }

        private void Initialize()
        {
        }

        private void Populate()
        {
            _movies = ServerRequest.LoadMovieList();


        }
    }
}

