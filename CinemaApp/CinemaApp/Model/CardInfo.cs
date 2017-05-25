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
    [Serializable]
    class CardInfo
    {
        public string Number { get; set; }
        public string ExpDate { get; set; }
        public string CVV { get; set; }
    }
}