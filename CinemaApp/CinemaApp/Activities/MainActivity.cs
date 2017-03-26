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

namespace CinemaApp.Activities
{
    [Activity(Label = "CinemaApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private LinearLayout _root;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView (Resource.Layout.Main);
            new Thread(Initialize).Start();
        }

        private void Initialize()
        {
            _root = FindViewById<LinearLayout>(Resource.Id.root);
            ServerRequest.LoadMovieList();
            RunOnUiThread(Populate);
        }

        private void Populate()
        {
            LinearLayout row = null;
            for (int i = 0; i < Schedule.Movies.Count; i++)
            {
                if (i % 2 == 0)
                    row = CreateNewRow();
                RelativeLayout container = CreateMovieContainer();
                ImageView poster = CreatePoster(Schedule.Movies[i].Poster);
                TextView title = CreateTitle(Schedule.Movies[i].Title);
                container.AddView(poster);
                container.AddView(title);
                container.Click += Movie_Click;
                row?.AddView(container);
                if (i % 2 == 1 || i == Schedule.Movies.Count - 1)
                    _root.AddView(row);
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

        private LinearLayout CreateNewRow()
        {
            LinearLayout row = new LinearLayout(this);
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
                                                                                   ViewGroup.LayoutParams.WrapContent);
            layoutParams.Gravity = GravityFlags.CenterHorizontal;
            row.Orientation = Orientation.Horizontal;
            row.LayoutParameters = layoutParams;
            return row;
        }

        private RelativeLayout CreateMovieContainer()
        {
            RelativeLayout container = new RelativeLayout(this);
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(420, 810);
            layoutParams.LeftMargin = layoutParams.RightMargin = 60;
            layoutParams.TopMargin = layoutParams.BottomMargin = 30;
            container.LayoutParameters = layoutParams;
            return container;
        }

        private ImageView CreatePoster(byte[] posterBytes)
        {
            ImageView img = new ImageView(this);
            img.SetImageBitmap(BitmapFactory.DecodeByteArray(posterBytes, 0, posterBytes.Length));
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
            RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
                                                                                       ViewGroup.LayoutParams.WrapContent);
            layoutParams.AddRule(LayoutRules.AlignParentBottom);
            layoutParams.AddRule(LayoutRules.CenterInParent);
            titleField.LayoutParameters = layoutParams;
            return titleField;
        }
    }
}

