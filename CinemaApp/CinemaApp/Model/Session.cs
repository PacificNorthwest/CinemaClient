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
    class Session
    {
        public int ID { get; set; }
        public string Time { get; set; }
        public int Hall { get; set; }
        public List<Seat> BookedSeats { get; set; } = new List<Seat>();
    }
}