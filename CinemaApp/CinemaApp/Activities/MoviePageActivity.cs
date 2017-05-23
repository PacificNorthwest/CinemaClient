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
using Android.Views.Animations;
using Android.Animation;

namespace CinemaApp.Activities
{
    [Activity(Label = "MoviePageActivity")]
    public partial class MoviePageActivity : Activity
    {
        private Model.Movie _movie;
        private LinearLayout _bookingPage;
        private LinearLayout _mainContainer;
        private ImageView _background;
        private ImageView _mask;
        private ImageView _trailerButton;
        private TextView _textViewTitle;
        private TextView _textViewDetails;
        private TextView _description;
        private Button _buttonBookTicket;

        private Bitmap _overlay;
        private Bitmap _poster;
        private Bitmap _thumbnail;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.RequestFeature(WindowFeatures.ContentTransitions);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MoviePageLayout);
            _movie = Schedule.GetMovieByID(Intent.GetIntExtra("MovieID", 0));
            FindViews();
            Initialize();
            Animate();
        }

        protected override void OnDestroy()
        {
            _overlay.Recycle();
            _poster.Recycle();
            _thumbnail.Recycle();
            _overlay = _poster = _thumbnail = null;
            base.OnDestroy();
        }

        private void Initialize()
        {
            _textViewTitle.PaintFlags = _textViewTitle.PaintFlags | PaintFlags.UnderlineText;
            _textViewTitle.Text = _movie.Title;
            _textViewDetails.Text = $"Страна: {_movie.Country}\nРежиссер: {_movie.Director}\nВремя: {_movie.Length} мин.\nЖанр: {_movie.Genres}\nРейтинг IMDb: {_movie.IMDbRating}";
            SpannableString ss = new SpannableString($"Описание:\n{_movie.Description}");
            ss.SetSpan(new DescriptionLeadingMarginSpan2(8, 350), 0, ss.Length(), 0);
            _description.TextFormatted = ss;

            _poster = BitmapFactory.DecodeByteArray(_movie.Poster, 0, _movie.Poster.Length);
            _background.SetImageBitmap(_poster);
            _mask.SetImageBitmap(CreateGradientMask(_poster.Height, _poster.Width));
            _trailerButton.SetImageBitmap(CreateTrailerThumbnail(_poster));

            _trailerButton.Click += (object sender, EventArgs e) => 
                { StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse(_movie.Trailer))); };
            _buttonBookTicket.Click += (object sender, EventArgs e) => { RevealBookingPage(); };
        }

        private void FindViews()
        {
            _mainContainer = FindViewById<LinearLayout>(Resource.Id.mainContainer);
            _background = FindViewById<ImageView>(Resource.Id.background);
            _mask = FindViewById<ImageView>(Resource.Id.mask);
            _trailerButton = FindViewById<ImageView>(Resource.Id.trailerButton);
            _textViewTitle = FindViewById<TextView>(Resource.Id.textViewTitle);
            _textViewDetails = FindViewById<TextView>(Resource.Id.textViewDetails);
            _description = FindViewById<TextView>(Resource.Id.description);
            _buttonBookTicket = FindViewById<Button>(Resource.Id.buttonBookTicket);

            _bookingPage = FindViewById<LinearLayout>(Resource.Id.bookingPage);
            _bookingPageMovieTitle = FindViewById<TextView>(Resource.Id.bookingPageMovieTitle);
            _dateSpinner = FindViewById<Spinner>(Resource.Id.dateSpinner);
            _sessionsGrid = FindViewById<GridLayout>(Resource.Id.sessionsGrid);
            _seatsScheme = FindViewById<GridLayout>(Resource.Id.seats);
        }

        private Bitmap CreateGradientMask(int height, int width)
        {
            _overlay = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(_overlay);
            Paint paint = new Paint();
            LinearGradient shader = new LinearGradient(0, 0, 0, height, 
                                                       new int[] { Color.Transparent, Color.Transparent, Color.ParseColor("#80000000"), Color.Black },
                                                       new float[] { 0, .2f, .40f, 1},
                                                       Shader.TileMode.Clamp);
            paint.SetShader(shader);
            canvas.DrawRect(0, 0, width, height, paint);
            return _overlay;
        }

        private Bitmap CreateTrailerThumbnail(Bitmap poster)
        {
            _thumbnail = Bitmap.CreateBitmap(poster.Width, poster.Height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(_thumbnail);
            canvas.DrawBitmap(poster, 0, 0, null);
            Paint paint = new Paint();
            paint.Color = Color.ParseColor("#4D000000");
            canvas.DrawRect(new Rect(0, 0, poster.Width, poster.Height), paint);
            canvas.DrawBitmap(BitmapFactory.DecodeResource(Resources, Resource.Drawable.videoIcon),
                              null,
                              new Rect(poster.Width/13, poster.Height/5, (poster.Width - poster.Width/13), (poster.Height - poster.Height/5)),
                              null);
            return _thumbnail;
        }

        private void Animate()
        {
            Window.EnterTransition = new Fade();
            Window.SharedElementsUseOverlay = false;
        }

        private void RevealBookingPage()
        {
            if (_bookingPage.Visibility == ViewStates.Invisible)
            {
                InitializeBookingPage();
                int centerX = (_buttonBookTicket.Left + _buttonBookTicket.Right) / 2;
                int centerY = (_buttonBookTicket.Top + _buttonBookTicket.Bottom) / 2;
                float radius = Math.Max(_mainContainer.Width, _mainContainer.Height);

                _bookingPage.Visibility = ViewStates.Visible;
                Animator show = ViewAnimationUtils.CreateCircularReveal(_bookingPage, centerX, centerY, 0, radius);
                show.AnimationEnd += (object sender, EventArgs e) => { _mainContainer.Visibility = ViewStates.Invisible; };
                show.SetDuration(600);
                show.Start();
            }
        }
    }
}