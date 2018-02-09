using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using RoutingEffects = XamarinCommunityToolkit.Effects;
using PlatformEffects = XamarinCommunityToolkit.Effects.Droid;

[assembly: ExportEffect(typeof(PlatformEffects.EntryClear), nameof(RoutingEffects.EntryClear))]
namespace XamarinCommunityToolkit.Effects.Droid
{
    [Preserve(AllMembers = true)]
    public class EntryClear : PlatformEffect
    {
        protected override void OnAttached()
        {
            ConfigureControl();
        }

        protected override void OnDetached()
        {
            var editText = Control as EditText;

            editText?.SetOnTouchListener(null);
            editText.SetCompoundDrawablesRelativeWithIntrinsicBounds(0, 0, 0, 0);
        }

        private void ConfigureControl()
        {
            var editText = Control as EditText;
            if (editText == null)
                return;

            editText.SetCompoundDrawablesRelativeWithIntrinsicBounds(0, 0, XamarinCommunityToolkit.Effects.Droid.Resource.Drawable.fct_ic_clear_icon, 0);
            editText.SetOnTouchListener(new OnDrawableTouchListener());
        }
    }

    class OnDrawableTouchListener : Java.Lang.Object, Android.Views.View.IOnTouchListener
    {
        public bool OnTouch(Android.Views.View v, MotionEvent e)
        {
            if (v is EditText && e.Action == MotionEventActions.Up)
            {
                var editText = (EditText)v;
                if (e.RawX >= (editText.Right - editText.GetCompoundDrawables()[2].Bounds.Width()))
                {
                    editText.Text = string.Empty;
                    return true;
                }
            }

            return false;
        }
    }
}