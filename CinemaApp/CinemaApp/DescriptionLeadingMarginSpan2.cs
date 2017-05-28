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
using Android.Text.Style;
using Android.Graphics;
using Android.Text;
using Java.Lang;

namespace CinemaApp
{
    class DescriptionLeadingMarginSpan2 : Java.Lang.Object, ILeadingMarginSpanLeadingMarginSpan2
    {
        private int _margin;
        private int _lines;

        public DescriptionLeadingMarginSpan2(int lines, int margin)
        {
            _lines = lines;
            _margin = margin;
        }

        public int LeadingMarginLineCount => _lines;

        public void DrawLeadingMargin(Canvas c, Paint p, 
                                      int x, int dir, int top, int baseline, int bottom,
                                      ICharSequence text,
                                      int start, int end, bool first,
                                      Layout layout) { }

        public int GetLeadingMargin(bool first) => (first) ? _margin : 30;
    }
}