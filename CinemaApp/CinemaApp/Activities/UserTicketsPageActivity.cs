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
using CinemaApp.Model;
using ZXing.QrCode;
using Android.Graphics;
using ZXing.Mobile;

namespace CinemaApp.Activities
{
    [Activity(Label = "UserTicketsPageActivity")]
    public class UserTicketsPageActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.UserTicketsPageLayout);
            Populate();
        }

        private void Populate()
        {
            var panel = FindViewById<LinearLayout>(Resource.Id.ticketsPanel);
            var qr = new BarcodeWriter
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                { Height = 800, Width = 800 }
            };

            foreach (var ticket in Schedule.BookedTickets)
            {
                ImageView image = new ImageView(this);
                image.SetImageBitmap(qr.Write($"https://cinemaserver.azurewebsites.net/api/booking/{ticket}"));
                LinearLayout.LayoutParams parameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
                                                                                     ViewGroup.LayoutParams.WrapContent);
                parameters.SetMargins(10, 50, 10, 10);
                image.LayoutParameters = parameters;
                panel.AddView(image);
            }
        }
    }
}