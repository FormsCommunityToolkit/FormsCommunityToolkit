﻿using System.Threading.Tasks;
using Xamarin.Forms;

namespace Microsoft.Toolkit.Xamarin.Forms.Behaviors
{
    public class RotateAnimation : AnimationBase
    {
        public static readonly BindableProperty RotationProperty =
           BindableProperty.Create(nameof(Rotation), typeof(double), typeof(AnimationBase), 180.0, BindingMode.TwoWay, defaultValueCreator: GetDefaulRotationProperty);

        public double Rotation
        {
            get => (double)GetValue(RotationProperty); 
            set => SetValue(RotationProperty, value); 
        }

        static object GetDefaulRotationProperty(BindableObject bindable)
            => ((RotateAnimation)bindable).DefaultRotation;

        protected override uint DefaultDuration { get; set; } = 200;
        protected virtual double DefaultRotation { get; set; } = 180.0;

        public override async Task Animate(View view)
        {
            var easing = AnimationHelper.GetEasing(Easing);
            await view.RotateTo(Rotation, Duration, easing);
            view.Rotation = 0;
        }
    }
}
