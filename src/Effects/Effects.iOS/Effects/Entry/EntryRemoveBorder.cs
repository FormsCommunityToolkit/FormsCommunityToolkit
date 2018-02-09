﻿using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using Foundation;
using RoutingEffects = XamarinCommunityToolkit.Effects;
using PlatformEffects = XamarinCommunityToolkit.Effects.iOS;

[assembly: ExportEffect(typeof(PlatformEffects.EntryRemoveBorder), nameof(RoutingEffects.EntryRemoveBorder))]
namespace XamarinCommunityToolkit.Effects.iOS
{
    [Preserve(AllMembers = true)]
    public class EntryRemoveBorder : PlatformEffect
    {
        private UITextBorderStyle _old;

        protected override void OnAttached()
        {
            var editText = Control as UITextField;
            if (editText == null)
                return;

            _old = editText.BorderStyle;
            editText.BorderStyle = UITextBorderStyle.None;
        }

        protected override void OnDetached()
        {
            var editText = Control as UITextField;
            if (editText == null)
                return;

            editText.BorderStyle = _old;
        }
    }
}
