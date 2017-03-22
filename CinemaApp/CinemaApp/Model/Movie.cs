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

namespace CinemaApp.Model
{
    class Movie
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Director { get; set; }
        public bool Is3D { get; set; }
        public List<Day> ShowDays { get; set; } = new List<Day>();
        public byte[] Poster { get; set; }
    }
}