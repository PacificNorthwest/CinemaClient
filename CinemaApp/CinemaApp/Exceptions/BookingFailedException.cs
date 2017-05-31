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

namespace CinemaApp.Exceptions
{
    class BookingFailedException :Exception
    {
        public new string Message => "Booking failed!";
        public override string ToString() => Message;
    }
}