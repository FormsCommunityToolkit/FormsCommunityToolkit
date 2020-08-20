﻿using System;
using System.Globalization;
using Xamarin.Forms;

using TizenLabel = Xamarin.Forms.Platform.Tizen.Native.Label;
using XLabel = Xamarin.Forms.Label;
using XTextAlignment = Xamarin.Forms.TextAlignment;
using PlayerState = Xamarin.CommunityToolkit.Tizen.UI.Views.PlaybackState;

namespace Xamarin.CommunityToolkit.Tizen.UI.Views
{
	public class EmbeddingControls : ContentView
	{
		public ImageButton PlayImage { get; private set; }
		public ImageButton PauseImage { get; private set; }

		public EmbeddingControls()
		{
			PlayImage = new ImageButton
			{
				Source = ImageSource.FromResource(ThemeConstants.MediaPlayer.Resources.PlayImagePath, typeof(EmbeddingControls).Assembly),
				IsVisible = false
			};
			PlayImage.Clicked += OnImageButtonClicked;
			AbsoluteLayout.SetLayoutFlags(PlayImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(PlayImage, new Rectangle(0.5, 0.5, 0.25, 0.25));

			PauseImage = new ImageButton
			{
				Source = ImageSource.FromResource(ThemeConstants.MediaPlayer.Resources.PauseImagePath, typeof(EmbeddingControls).Assembly),
				IsVisible = false
			};
			PauseImage.Clicked += OnImageButtonClicked;
			AbsoluteLayout.SetLayoutFlags(PauseImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(PauseImage, new Rectangle(0.5, 0.5, 0.25, 0.25));

			var bufferingLabel = new XLabel
			{
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(TizenLabel), false),
				HorizontalTextAlignment = XTextAlignment.Center,
				TextColor = ThemeConstants.MediaPlayer.ColorClass.DefaultProgressLabelColor
			};
			bufferingLabel.SetBinding(XLabel.TextProperty, new Binding
			{
				Path = "BufferingProgress",
				StringFormat = "{0:0%}"
			});
			bufferingLabel.SetBinding(IsVisibleProperty, new Binding
			{
				Path = "IsBuffering",
			});
			AbsoluteLayout.SetLayoutFlags(bufferingLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(bufferingLabel, new Rectangle(0.5, 0.5, 0.25, 0.25));

			var progressBoxView = new BoxView
			{
				Color = ThemeConstants.MediaPlayer.ColorClass.DefaultProgressBarColor
			};
			progressBoxView.SetBinding(AbsoluteLayout.LayoutBoundsProperty, new Binding
			{
				Path = "Progress",
				Converter = new ProgressToBoundTextConverter()
			});
			AbsoluteLayout.SetLayoutFlags(progressBoxView, AbsoluteLayoutFlags.All);

			var posLabel = new XLabel
			{
				Margin = new Thickness(10, 0, 0, 0),
				FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(XLabel)),
				HorizontalTextAlignment = XTextAlignment.Start
			};
			posLabel.SetBinding(XLabel.TextProperty, new Binding
			{
				Path = "Position",
				Converter = new MillisecondToTextConverter()
			});
			AbsoluteLayout.SetLayoutFlags(posLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(posLabel, new Rectangle(0, 0, 1, 1));

			var durationLabel = new XLabel
			{
				Margin = new Thickness(0, 0, 10, 0),
				FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(XLabel)),
				HorizontalTextAlignment = XTextAlignment.End
			};
			durationLabel.SetBinding(XLabel.TextProperty, new Binding
			{
				Path = "Duration",
				Converter = new MillisecondToTextConverter()
			});
			AbsoluteLayout.SetLayoutFlags(durationLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(durationLabel, new Rectangle(0, 0, 1, 1));

			var progressInnerLayout = new AbsoluteLayout
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 23,
				BackgroundColor = ThemeConstants.MediaPlayer.ColorClass.DefaultProgressAreaColor,
				Children =
				{
					progressBoxView,
					posLabel,
					durationLabel
				}
			};

			var progressLayout = new StackLayout
			{
				Children =
				{
					new StackLayout { VerticalOptions = LayoutOptions.FillAndExpand },
					new StackLayout
					{
						Margin =  Device.Idiom == TargetIdiom.Watch ? new Thickness(80, 0, 80, 0) : 20,
						VerticalOptions = LayoutOptions.End,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						BackgroundColor = ThemeConstants.MediaPlayer.ColorClass.DefaultProgressAreaBackgroundColor,
						Children = { progressInnerLayout }
					}
				}
			};
			AbsoluteLayout.SetLayoutFlags(progressLayout, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(progressLayout, new Rectangle(0, 0, 1, 1));

			Content = new AbsoluteLayout
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = {
					progressLayout,
					PlayImage,
					PauseImage,
					bufferingLabel
				}
			};
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();
			if (BindingContext is IMediaPlayer player)
			{
				player.PlaybackPaused += OnPlaybackStateChanged;
				player.PlaybackStarted += OnPlaybackStateChanged;
				player.PlaybackStopped += OnPlaybackStateChanged;
			}
		}

		async void OnPlaybackStateChanged(object sender, EventArgs e)
		{
			if (BindingContext is IMediaPlayer player)
			{
				if (player.State == PlayerState.Playing)
				{
					var unused = PlayImage.FadeTo(0, 100);
					await PlayImage.ScaleTo(3.0, 300);
					PlayImage.IsVisible = false;
					PlayImage.Scale = 1.0;

					PauseImage.IsVisible = true;
					unused = PauseImage.FadeTo(1, 50);
				}
				else
				{
					var unused = PauseImage.FadeTo(0, 100);
					await PauseImage.ScaleTo(3.0, 300);
					PauseImage.IsVisible = false;
					PauseImage.Scale = 1.0;

					PlayImage.IsVisible = true;
					unused = PlayImage.FadeTo(1, 50);
				}
			}
		}

		async void OnImageButtonClicked(object sender, EventArgs e)
		{
			if (BindingContext is IMediaPlayer player)
			{
				if (player.State == PlayerState.Playing)
				{
					player.Pause();
				}
				else
				{
					await player.Start();
				}
			}
		}
	}

	public class ProgressToBoundTextConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var progress = (double)value;
			if (double.IsNaN(progress))
			{
				progress = 0d;
			}
			return new Rectangle(0, 0, progress, 1);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var rect = (Rectangle)value;
			return rect.Width;
		}
	}

	public class MillisecondToTextConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var millisecond = (int)value;
			var second = (millisecond / 1000) % 60;
			var min = (millisecond / 1000 / 60) % 60;
			var hour = (millisecond / 1000 / 60 / 60);
			if (hour > 0)
			{
				return string.Format("{0:d2}:{1:d2}:{2:d2}", hour, min, second);
			}
			else
			{
				return string.Format("{0:d2}:{1:d2}", min, second);
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}
}
