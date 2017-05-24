using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace CinemaApp.Resources.views
{
    public class SeatButton : Button
    {
        private bool _checked;
        public bool Checked
        {
            get { return _checked; }
            set { _checked = value;
                          if (value) this.SetBackgroundColor(Android.Graphics.Color.LightGreen);
                          else this.SetBackgroundColor(Android.Graphics.Color.White); }
        }

        public SeatButton(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public SeatButton(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Click += (object sender, EventArgs e) => Checked = !Checked;
        }
    }
}