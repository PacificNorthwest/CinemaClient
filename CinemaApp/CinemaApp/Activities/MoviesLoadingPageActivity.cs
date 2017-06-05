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
using CinemaApp.Server;
using System.Threading;

namespace CinemaApp.Activities
{
    [Activity(Label = "MoviesLoadingPageActivity")]
    public class MoviesLoadingPageActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MoviesLoadingPageActivity);
            new Thread(Load).Start();
        }

        private void Load()
        {
            var token = Security.SecurityProvider.GetUserToken();
            ServerRequest.LoadInfo(token);
            foreach (Model.Movie movie in Schedule.Movies)
            {
                movie.BitmapPoster = BitmapFactory.DecodeByteArray(movie.Poster, 0, movie.Poster.Length);
                movie.TrailerThumbnail = CreateTrailerThumbnail(movie.BitmapPoster);
                movie.GradientMask = CreateGradientMask(movie.BitmapPoster.Height, movie.BitmapPoster.Width);
            }
            RunOnUiThread(() => StartActivity(new Intent(this, typeof(MainActivity))));
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