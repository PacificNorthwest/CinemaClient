using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using CinemaApp.Server;
using System;
using Android.Views;
using System.IO;
using Android.Graphics;
using System.Threading;
using Newtonsoft.Json;
using Android.Content;
using CinemaApp.Model;
using Java.IO;
using Android.Transitions;

namespace CinemaApp.Activities
{
    [Activity(Label = "CinemaApp")]
    public class MainActivity : Activity
    {
        private GridLayout _root;

        protected override void OnCreate(Bundle bundle)
        {
            Window.RequestFeature(WindowFeatures.ContentTransitions);
            base.OnCreate(bundle);
            SetContentView (Resource.Layout.Main);
            new Thread(Initialize).Start();
            Animate();
        }

        private void Animate()
        {
           //Window.ExitTransition = new Fade();
        }

        private void Initialize()
        {
            _root = FindViewById<GridLayout>(Resource.Id.root);
            if (Schedule.Movies == null)
            {
                ServerRequest.LoadMovieList();
                foreach (Model.Movie movie in Schedule.Movies)
                {
                    movie.BitmapPoster = BitmapFactory.DecodeByteArray(movie.Poster, 0, movie.Poster.Length);
                    movie.TrailerThumbnail = CreateTrailerThumbnail(movie.BitmapPoster);
                    movie.GradientMask = CreateGradientMask(movie.BitmapPoster.Height, movie.BitmapPoster.Width);
                }
            }
            RunOnUiThread(Populate);
        }

        private void Populate()
        {
            foreach (Model.Movie movie in Schedule.Movies)
            { 
                RelativeLayout container = CreateMovieContainer();
                ImageView poster = CreatePosterContainer(movie.BitmapPoster);
                TextView title = CreateTitle(movie.Title);
                container.AddView(poster);
                container.AddView(title);
                container.Click += Movie_Click;
                _root.AddView(container);
            }
            FindViewById<RelativeLayout>(Resource.Id.loadingPanel).Visibility = ViewStates.Gone;
        }

        private void Movie_Click(object sender, EventArgs e)
        {
            View poster = (sender as RelativeLayout).GetChildAt(0);
            string title = ((sender as RelativeLayout).GetChildAt(1) as TextView).Text;
            poster.TransitionName = "Poster";
            ActivityOptions options = ActivityOptions.MakeSceneTransitionAnimation(this, poster, "Poster");
            Intent intent = new Intent(this, typeof(MoviePageActivity));
            intent.PutExtra("MovieID", Schedule.GetMovieByTitle(title).ID);
            StartActivity(intent, options.ToBundle());
        }

        private RelativeLayout CreateMovieContainer()
        {
            RelativeLayout container = new RelativeLayout(this);
            GridLayout.LayoutParams layoutParams = new GridLayout.LayoutParams();
            layoutParams.SetGravity(GravityFlags.Center);
            layoutParams.Height = 810;
            layoutParams.Width = 420;
            layoutParams.LeftMargin = layoutParams.RightMargin = 60;
            layoutParams.TopMargin = layoutParams.BottomMargin = 30;
            container.LayoutParameters = layoutParams;
            return container;
        }

        private ImageView CreatePosterContainer(Bitmap poster)
        {
            ImageView img = new ImageView(this);
            img.SetImageBitmap(poster);
            RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, 720);
            layoutParams.AddRule(LayoutRules.AlignParentTop);
            img.LayoutParameters = layoutParams;
            return img;
        }

        private TextView CreateTitle(string title)
        {
            TextView titleField = new TextView(this);
            titleField.Text = title;
            titleField.TextSize = 21;
            titleField.SetTextColor(Color.White);
            titleField.TextAlignment = TextAlignment.Center;
            RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
                                                                                       ViewGroup.LayoutParams.WrapContent);
            layoutParams.AddRule(LayoutRules.AlignParentBottom);
            layoutParams.AddRule(LayoutRules.CenterInParent);
            titleField.LayoutParameters = layoutParams;
            return titleField;
        }

        private Bitmap CreateGradientMask(int height, int width)
        {
            Bitmap overlay = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(overlay);
            Paint paint = new Paint();
            LinearGradient shader = new LinearGradient(0, 0, 0, height,
                                                       new int[] { Color.Transparent, Color.Transparent, Color.ParseColor("#80000000"), Color.Black },
                                                       new float[] { 0, .2f, .40f, 1 },
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
                              new Rect(poster.Width / 13, poster.Height / 5, (poster.Width - poster.Width / 13), (poster.Height - poster.Height / 5)),
                              null);
            return thumbnail;
        }
    }
}

