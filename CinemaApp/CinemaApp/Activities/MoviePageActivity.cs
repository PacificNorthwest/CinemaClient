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
using CinemaApp.Resources.views;

namespace CinemaApp.Activities
{
    [Activity(Label = "MoviePageActivity")]
    public partial class MoviePageActivity : Activity
    {
        private Model.Movie _movie;
        private ScrollView _bookingPage;
        private LinearLayout _mainContainer;
        private ImageView _background;
        private ImageView _mask;
        private ImageView _trailerButton;
        private TextView _textViewTitle;
        private TextView _textViewDetails;
        private TextView _seatsList;
        private TextView _totalPrice;
        private ExpandableTextView _description;
        private Button _buttonBookTicket;
        private Button _buttonConfirmBooking;

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

        private void Initialize()
        {
            _textViewTitle.PaintFlags = _textViewTitle.PaintFlags | PaintFlags.UnderlineText;
            _textViewTitle.Text = _movie.Title;
            _textViewDetails.Text = $"Country: {_movie.Country}\nDirector: {_movie.Director}\nDuration: {_movie.Length} мин.\nGenres: {_movie.Genres}\nIMDB rating: {_movie.IMDbRating}";
            SpannableString ss = new SpannableString($"Description:\n{_movie.Description}");
            ss.SetSpan(new DescriptionLeadingMarginSpan2(8, 350), 0, ss.Length(), 0);
            _description.PutText(ss);

            _background.SetImageBitmap(_movie.BitmapPoster);
            _mask.SetImageBitmap(_movie.GradientMask);
            _trailerButton.SetImageBitmap(_movie.TrailerThumbnail);

            _trailerButton.Click += (object sender, EventArgs e) => 
                 StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse(_movie.Trailer)));
            _buttonBookTicket.Click += (object sender, EventArgs e) => RevealBookingPage();
            
            try
            {
                InitializeBookingPage();
            }
            catch { }
        }

        private void FindViews()
        {
            _mainContainer = FindViewById<LinearLayout>(Resource.Id.mainContainer);
            _background = FindViewById<ImageView>(Resource.Id.background);
            _mask = FindViewById<ImageView>(Resource.Id.mask);
            _trailerButton = FindViewById<ImageView>(Resource.Id.trailerButton);
            _textViewTitle = FindViewById<TextView>(Resource.Id.textViewTitle);
            _textViewDetails = FindViewById<TextView>(Resource.Id.textViewDetails);
            _description = FindViewById<ExpandableTextView>(Resource.Id.description);
            _buttonBookTicket = FindViewById<Button>(Resource.Id.buttonBookTicket);

            _bookingPage = FindViewById<ScrollView>(Resource.Id.bookingPage);
            _bookingPageMovieTitle = FindViewById<TextView>(Resource.Id.bookingPageMovieTitle);
            _dateSpinner = FindViewById<Spinner>(Resource.Id.dateSpinner);
            _sessionsGrid = FindViewById<GridLayout>(Resource.Id.sessionsGrid);
            _seatsScheme = FindViewById<GridLayout>(Resource.Id.seats);
            _seatsList = FindViewById<TextView>(Resource.Id.seatslist);
            _totalPrice = FindViewById<TextView>(Resource.Id.totalPrice);
            _buttonConfirmBooking = FindViewById<Button>(Resource.Id.buttonConfirmBooking);
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