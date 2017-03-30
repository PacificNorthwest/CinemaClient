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

namespace CinemaApp.ExtensionMethods
{
    static class Extensions
    {
        public static Bitmap AddGradient(this Bitmap src)
        {
            int width = src.Width;
            int height = src.Height;
            Bitmap overlay = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(overlay);

            canvas.DrawBitmap(src, 0, 0, null);
            Paint paint = new Paint();
            LinearGradient shader = new LinearGradient(0, 0, 0, height, Color.ParseColor("#00000000"), 
                                                                        Color.Black,
                                                                        Shader.TileMode.Clamp);
            paint.SetShader(shader);
            canvas.DrawRect(0, 0, width, height, paint);
            return overlay;
        }
    }
}