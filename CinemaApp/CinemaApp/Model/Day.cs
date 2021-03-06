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
    class Day
    {
        public DateTime Date { get; set; }
        public List<Session> Sessions { get; set; } = new List<Session>();

        public override string ToString() => Date.ToString("dd.MM");
    }
}