using Android.Graphics;
using System;
using System.Collections.Generic;

namespace CinemaApp.Model
{
    class Movie
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Director { get; set; }
        public string Country { get; set; }
        public int Length { get; set; }
        public string Genres { get; set; }
        public string Description { get; set; }
        public string Trailer { get; set; }
        public bool Is3D { get; set; }
        public string IMDbRating { get; set; }
        public List<Day> ShowDays { get; set; } = new List<Day>();
        public byte[] Poster { get; set; }

        public Bitmap BitmapPoster { get; set; }
        public Bitmap TrailerThumbnail { get; set; }
        public Bitmap GradientMask { get; set; }
    }
}