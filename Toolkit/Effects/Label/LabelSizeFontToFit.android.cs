﻿using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Runtime;
using Android.Widget;
using Android.Graphics;
using Android.Util;
using Android.Content;
using RoutingEffects = Xamarin.Toolkit.Effects;
using PlatformEffects = Xamarin.Toolkit.Effects.Droid;

[assembly: ExportEffect(typeof(PlatformEffects.LabelSizeFontToFit), nameof(RoutingEffects.LabelSizeFontToFit))]
namespace Xamarin.Toolkit.Effects.Droid
{
    class ShrinkTextOnLayoutChangeListener : Java.Lang.Object, global::Android.Views.View.IOnLayoutChangeListener
    {
        const string TextMeasure = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        const float Threshold = 0.5f; // How close we have to be

        private readonly TextView _textView;

        public ShrinkTextOnLayoutChangeListener(TextView textView) : base()
        {
            _textView = textView;
        }

        public void OnLayoutChange(global::Android.Views.View v, int left, int top, int right, int bottom, int oldLeft, int oldTop, int oldRight, int oldBottom)
        {
            if (_textView.Width <= 0 || _textView.Height <= 0) return;
            
            var hi = ConvertSpToPixels(_textView.TextSize, _textView.Context);
            var lo = 1f;

            var paint = new Paint(_textView.Paint);
            var bounds = new Rect();

            while ((hi - lo) > Threshold)
            {
                float size = (hi + lo) / 2;
                paint.TextSize = size;
                paint.GetTextBounds(TextMeasure, 0, TextMeasure.Length, bounds);

                if (paint.MeasureText(_textView.Text) >= _textView.Width || bounds.Height() >= _textView.Height)
                    hi = size; // too big
                else
                    lo = size; // too small
            }

            _textView.SetTextSize(ComplexUnitType.Px, lo);
        }

        static float ConvertSpToPixels(float sp, Context context) => TypedValue.ApplyDimension(ComplexUnitType.Px, sp, context.Resources.DisplayMetrics);
    }

    [Preserve(AllMembers = true)]
    public class LabelSizeFontToFit : PlatformEffect
    {
        private ShrinkTextOnLayoutChangeListener _listener;

        protected override void OnAttached()
        {
            var textView = Control as TextView;
            if (textView == null)
                return;

            textView.AddOnLayoutChangeListener(_listener = new ShrinkTextOnLayoutChangeListener(textView));
        }

        protected override void OnDetached()
        {
            var textView = Control as TextView;
            if (textView == null)
                return;

            textView.RemoveOnLayoutChangeListener(_listener);
        }
    }
}
