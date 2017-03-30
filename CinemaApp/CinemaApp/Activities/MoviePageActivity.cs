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
using Android.Graphics;
using CinemaApp.Model;
using Android.Transitions;
using CinemaApp.ExtensionMethods;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Text.Style;

namespace CinemaApp.Activities
{
    [Activity(Label = "MoviePageActivity")]
    public class MoviePageActivity : Activity
    {
        private Model.Movie _movie;
        private ImageView _background;
        private TextView _textViewTitle;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.RequestFeature(WindowFeatures.ContentTransitions);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MoviePageLayout);
            Initialize();
            Animate();
        }

        private void Initialize()
        {
            _movie = Schedule.GetMovieByID(Intent.GetIntExtra("MovieID", 0));
            _background = FindViewById<ImageView>(Resource.Id.background);
            _textViewTitle = FindViewById<TextView>(Resource.Id.textViewTitle);
            _textViewTitle.PaintFlags = _textViewTitle.PaintFlags | PaintFlags.UnderlineText;
            _textViewTitle.Text = _movie.Title;
            _background.SetImageBitmap(BitmapFactory.DecodeByteArray(_movie.Poster, 0, _movie.Poster.Length).AddGradient());
            
        }

        private void Animate()
        {
            Fade fade = new Fade();
            fade.SetDuration(500);
            Window.EnterTransition = fade;
        }
    }
}