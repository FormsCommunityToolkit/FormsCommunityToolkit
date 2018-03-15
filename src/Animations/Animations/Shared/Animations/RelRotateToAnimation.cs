﻿using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xamarin.Toolkit.Animations
{
    public class RelRotateToAnimation : AnimationBase
    {
        public static readonly BindableProperty RotationProperty =
            BindableProperty.Create(
                nameof(Rotation),
                typeof(double),
                typeof(RelRotateToAnimation),
                default(double),
                BindingMode.TwoWay,
                null);

        public double Rotation
        {
            get { return (double)GetValue(RotationProperty); }
            set { SetValue(RotationProperty, value); }
        }

        protected override Task BeginAnimation()
        {
            if (Target == null)
            {
                throw new NullReferenceException("Null Target property.");
            }

            return Target.RelRotateTo(Rotation, Convert.ToUInt32(Duration), EasingHelper.GetEasing(Easing));
        }
    }
}
