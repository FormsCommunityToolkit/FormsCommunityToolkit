using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Runtime;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using RoutingEffects = XamarinCommunityToolkit.Effects;
using PlatformEffects = XamarinCommunityToolkit.Effects.Droid;

[assembly: ExportEffect(typeof(PlatformEffects.EntryRemoveLine), nameof(RoutingEffects.EntryRemoveLine))]
namespace XamarinCommunityToolkit.Effects.Droid
{
    [Preserve(AllMembers = true)]
    public class EntryRemoveLine : PlatformEffect
    {
        protected override void OnAttached()
        {
            var shape = new ShapeDrawable(new RectShape());
            shape.Paint.Color = Android.Graphics.Color.Transparent;
            shape.Paint.StrokeWidth = 0;
            shape.Paint.SetStyle(Paint.Style.Stroke);
            Control.Background = shape;
        }

        protected override void OnDetached()
        {
        }
    }
}