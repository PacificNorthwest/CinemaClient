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
using Android.Text;
using Java.Lang;

namespace CinemaApp.Resources.views
{
    public class ExpandableTextView : TextView
    {
        private readonly int TRIM_LENGTH = 175;

        private ICharSequence _trimmedText;
        private ICharSequence _originalText;
        private bool _trim = true;
 

        public ExpandableTextView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public ExpandableTextView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Click += (object sender, EventArgs e) =>
                        {
                            _trim = !_trim;
                            if (_trim) this.TextFormatted = _trimmedText;
                            else this.TextFormatted = _originalText;
                        };
        }

        public void PutText(SpannableString originalText)
        {
            _originalText = originalText;
            _trimmedText = TextUtils.ConcatFormatted(_originalText.SubSequenceFormatted(0, TRIM_LENGTH),
                                                     new SpannableString("..."));
            this.TextFormatted = _trimmedText;
        }
    }
}