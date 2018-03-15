﻿using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xamarin.Toolkit.Animations
{
    public class TurnstileInAnimation : AnimationBase
    {
        protected override Task BeginAnimation()
        {
            if (Target == null)
            {
                throw new NullReferenceException("Null Target property.");
            }

            return Task.Run(() =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Target.Animate("TurnstileIn", TurnstileIn(), 16, Convert.ToUInt32(Duration));
                });
            });
        }

        internal Animation TurnstileIn()
        {
            var animation = new Animation();

            animation.WithConcurrent((f) => Target.RotationY = f, 75, 0, Xamarin.Forms.Easing.CubicOut);
            animation.WithConcurrent((f) => Target.Opacity = f, 0, 1, null, 0, 0.01);

            return animation;
        }
    }
}
