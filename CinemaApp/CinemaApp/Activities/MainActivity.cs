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
            Animate();
            Initialize();
        }

        private void Animate()
        {
           Window.EnterTransition = new Explode();
        }

        private void Initialize()
        {
            _root = FindViewById<GridLayout>(Resource.Id.root);
            Populate();
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
                Window.EnterTransition.AddTarget(container);
                _root.AddView(container);
            }
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

        
    }
}

