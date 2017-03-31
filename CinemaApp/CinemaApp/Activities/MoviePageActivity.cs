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
        private ImageView _mask;
        private ImageView _trailerButton;
        private TextView _textViewTitle;
        private TextView _textViewDetails;
        private TextView _description;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.RequestFeature(WindowFeatures.ContentTransitions);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MoviePageLayout);
            _movie = Schedule.GetMovieByID(Intent.GetIntExtra("MovieID", 0));
            Initialize();
            Animate();
        }

        private void Initialize()
        {
            _background = FindViewById<ImageView>(Resource.Id.background);
            _mask = FindViewById<ImageView>(Resource.Id.mask);
            _trailerButton = FindViewById<ImageView>(Resource.Id.trailerButton);
            _textViewTitle = FindViewById<TextView>(Resource.Id.textViewTitle);
            _textViewDetails = FindViewById<TextView>(Resource.Id.textViewDetails);
            _description = FindViewById<TextView>(Resource.Id.description);

            _textViewTitle.PaintFlags = _textViewTitle.PaintFlags | PaintFlags.UnderlineText;
            _textViewTitle.Text = _movie.Title;
            _textViewDetails.Text = $"Страна: {_movie.Country}\nРежиссер: {_movie.Director}\nВремя: {_movie.Length} мин.\nЖанр: {_movie.Genres}";
            _description.Text = $"Описание:\n{_movie.Description}";

            Bitmap poster = BitmapFactory.DecodeByteArray(_movie.Poster, 0, _movie.Poster.Length);
            _background.SetImageBitmap(poster);
            _mask.SetImageBitmap(CreateGradientMask(poster.Height, poster.Width));
            _trailerButton.SetImageBitmap(CreateTrailerThumbnail(poster));

            _trailerButton.Click += (object sender, EventArgs e) => 
                { StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse(_movie.Trailer))); };
        }

        private Bitmap CreateGradientMask(int height, int width)
        {
            Bitmap overlay = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(overlay);
            Paint paint = new Paint();
            LinearGradient shader = new LinearGradient(0, 0, 0, height, 
                                                       new int[] { Color.Transparent, Color.Transparent, Color.ParseColor("#80000000"), Color.Black },
                                                       new float[] { 0, .2f, .40f, 1},
                                                       Shader.TileMode.Clamp);
            paint.SetShader(shader);
            canvas.DrawRect(0, 0, width, height, paint);
            return overlay;
        }

        private Bitmap CreateTrailerThumbnail(Bitmap poster)
        {
            Bitmap thumbnail = Bitmap.CreateBitmap(poster.Width, poster.Height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(thumbnail);
            canvas.DrawBitmap(poster, 0, 0, null);
            Paint paint = new Paint();
            paint.Color = Color.ParseColor("#4D000000");
            canvas.DrawRect(new Rect(0, 0, poster.Width, poster.Height), paint);
            canvas.DrawBitmap(BitmapFactory.DecodeResource(Resources, Resource.Drawable.videoIcon),
                              null,
                              new Rect(poster.Width/13, poster.Height/5, (poster.Width - poster.Width/13), (poster.Height - poster.Height/5)),
                              null);
            return thumbnail;
        }

        private void Animate()
        {
            Fade fade = new Fade();
            fade.SetDuration(500);
            Window.EnterTransition = fade;
            Window.SharedElementsUseOverlay = false;
        }
    }
}