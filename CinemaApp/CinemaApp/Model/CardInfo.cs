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
    class CardInfo
    {
        public byte[] Number { get; set; }
        public byte[] ExpDate { get; set; }
        public byte[] CVV { get; set; }
    }
}