﻿using Android.Runtime;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Xamarin.Forms.Color;
using Picker = Android.Widget.EditText;
using RoutingEffects = XamarinCommunityToolkit.Effects;
using PlatformEffects = XamarinCommunityToolkit.Effects.Droid;

[assembly: ExportEffect(typeof(PlatformEffects.PickerChangeColor), nameof(RoutingEffects.PickerChangeColorEffect))]
namespace XamarinCommunityToolkit.Effects.Droid
{

    [Preserve (AllMembers = true)]
    public class PickerChangeColor : PlatformEffect
    {
        private Color _color;

        protected override void OnAttached ()
        {
            _color = (Color)Element.GetValue(RoutingEffects.PickerChangeColor.ColorProperty);
            ((Picker)Control).SetHintTextColor(_color.ToAndroid());
        }

        protected override void OnDetached ()
        {
        }
    }
}