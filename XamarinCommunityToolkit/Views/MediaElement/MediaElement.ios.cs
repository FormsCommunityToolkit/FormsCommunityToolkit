﻿using AVFoundation;
using AVKit;
using CoreMedia;
using Foundation;
using System;
using System.IO;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;
using CommunityToolkit = Xamarin.CommunityToolkit.UI.Views;

[assembly: ExportRenderer(typeof(CommunityToolkit::MediaElement), typeof(MediaElementRenderer))]
namespace Xamarin.CommunityToolkit.iOS.UI.Views
{
	[Foundation.Preserve(AllMembers = true)]
	public sealed class MediaElementRenderer : ViewRenderer<CommunityToolkit::MediaElement, UIView>
	{
		CommunityToolkit::IMediaElementController Controller => Element;

		readonly AVPlayerViewController avPlayerViewController = new AVPlayerViewController();
		NSObject playedToEndObserver;
		NSObject statusObserver;
		NSObject rateObserver;
		bool idleTimerDisabled = false;

		void SetKeepScreenOn(bool value)
		{
			if (value)
			{
				if (!UIApplication.SharedApplication.IdleTimerDisabled)
				{
					idleTimerDisabled = true;
					UIApplication.SharedApplication.IdleTimerDisabled = true;
				}
			}
			else if (idleTimerDisabled)
			{
				idleTimerDisabled = false;
				UIApplication.SharedApplication.IdleTimerDisabled = false;
			}
		}

		void UpdateSource()
		{
			if (Element.Source != null)
			{
				AVAsset asset = null;

				if (Element.Source is CommunityToolkit::UriMediaSource uriSource)
				{
					if (uriSource.Uri.Scheme == "ms-appx")
					{
						if (uriSource.Uri.LocalPath.Length <= 1)
							return;

						// used for a file embedded in the application package
						asset = AVAsset.FromUrl(NSUrl.FromFilename(uriSource.Uri.LocalPath.Substring(1)));
					}
					else if (uriSource.Uri.Scheme == "ms-appdata")
					{
						var filePath = ResolveMsAppDataUri(uriSource.Uri);

						if (string.IsNullOrEmpty(filePath))
							throw new ArgumentException("Invalid Uri", "Source");

						asset = AVAsset.FromUrl(NSUrl.FromFilename(filePath));
					}
					else
					{
						asset = AVUrlAsset.Create(NSUrl.FromString(uriSource.Uri.AbsoluteUri));
					}
				}
				else
				{
					if (Element.Source is CommunityToolkit::FileMediaSource fileSource)
					{
						asset = AVAsset.FromUrl(NSUrl.FromFilename(fileSource.File));
					}
				}

				var item = new AVPlayerItem(asset);
				RemoveStatusObserver();

				statusObserver = (NSObject)item.AddObserver("status", NSKeyValueObservingOptions.New, ObserveStatus);


				if (avPlayerViewController.Player != null)
				{
					avPlayerViewController.Player.ReplaceCurrentItemWithPlayerItem(item);
				}
				else
				{
					avPlayerViewController.Player = new AVPlayer(item);
					rateObserver = (NSObject)avPlayerViewController.Player.AddObserver("rate", NSKeyValueObservingOptions.New, ObserveRate);
				}

				if (Element.AutoPlay)
					Play();
			}
			else
			{
				if (Element.CurrentState == CommunityToolkit::MediaElementState.Playing || Element.CurrentState == CommunityToolkit::MediaElementState.Buffering)
				{
					avPlayerViewController.Player.Pause();
					Controller.CurrentState = CommunityToolkit::MediaElementState.Stopped;
				}
			}
		}

		string ResolveMsAppDataUri(Uri uri)
		{
			if (uri.Scheme == "ms-appdata")
			{
				string filePath;

				if (uri.LocalPath.StartsWith("/local"))
				{
					var libraryPath = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User)[0].Path;
					filePath = Path.Combine(libraryPath, uri.LocalPath.Substring(7));
				}
				else if (uri.LocalPath.StartsWith("/temp"))
				{
					filePath = Path.Combine(Path.GetTempPath(), uri.LocalPath.Substring(6));
				}
				else
				{
					throw new ArgumentException("Invalid Uri", "Source");
				}

				return filePath;
			}
			else
			{
				throw new ArgumentException("uri");
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (playedToEndObserver != null)
			{
				NSNotificationCenter.DefaultCenter.RemoveObserver(playedToEndObserver);
				playedToEndObserver = null;
			}

			if (rateObserver != null)
			{
				rateObserver.Dispose();
				rateObserver = null;
			}

			RemoveStatusObserver();

			avPlayerViewController?.Player?.ReplaceCurrentItemWithPlayerItem(null);

			base.Dispose(disposing);
		}

		void RemoveStatusObserver()
		{
			if (statusObserver != null)
			{
				try
				{
					avPlayerViewController?.Player?.CurrentItem?.RemoveObserver(statusObserver, "status");
				}
				finally
				{

					statusObserver = null;
				}
			}
		}

		void ObserveRate(NSObservedChange e)
		{
			if (Controller is object)
			{
				switch (avPlayerViewController.Player.Rate)
				{
					case 0.0f:
						Controller.CurrentState = CommunityToolkit::MediaElementState.Paused;
						break;

					case 1.0f:
						Controller.CurrentState = CommunityToolkit::MediaElementState.Playing;
						break;
				}

				Controller.Position = Position;
			}
		}

		void ObserveStatus(NSObservedChange e)
		{
			Controller.Volume = avPlayerViewController.Player.Volume;

			switch (avPlayerViewController.Player.Status)
			{
				case AVPlayerStatus.Failed:
					Controller.OnMediaFailed();
					break;

				case AVPlayerStatus.ReadyToPlay:
					var duration = avPlayerViewController.Player.CurrentItem.Duration;

					if (duration.IsIndefinite)
						Controller.Duration = TimeSpan.Zero;
					else
						Controller.Duration = TimeSpan.FromSeconds(duration.Seconds);

					Controller.VideoHeight = (int)avPlayerViewController.Player.CurrentItem.Asset.NaturalSize.Height;
					Controller.VideoWidth = (int)avPlayerViewController.Player.CurrentItem.Asset.NaturalSize.Width;
					Controller.OnMediaOpened();
					Controller.Position = Position;
					break;
			}
		}

		TimeSpan Position
		{
			get
			{
				if (avPlayerViewController.Player.CurrentTime.IsInvalid)
					return TimeSpan.Zero;

				return TimeSpan.FromSeconds(avPlayerViewController.Player.CurrentTime.Seconds);
			}
		}

		void PlayedToEnd(NSNotification notification)
		{
			if (Element == null)
			{
				return;
			}

			if (Element.IsLooping)
			{
				avPlayerViewController.Player.Seek(CMTime.Zero);
				Controller.Position = Position;
				avPlayerViewController.Player.Play();
			}
			else
			{
				SetKeepScreenOn(false);
				Controller.Position = Position;

				try
				{
					Device.BeginInvokeOnMainThread(Controller.OnMediaEnded);
				}
				catch (Exception e)
				{
					Log.Warning("MediaElement", $"Failed to play media to end: {e}");
				}
			}
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(CommunityToolkit::MediaElement.Aspect):
					avPlayerViewController.VideoGravity = AspectToGravity(Element.Aspect);
					break;

				case nameof(CommunityToolkit::MediaElement.KeepScreenOn):
					if (!Element.KeepScreenOn)
					{
						SetKeepScreenOn(false);
					}
					else if (Element.CurrentState == CommunityToolkit::MediaElementState.Playing)
					{
						// only toggle this on if property is set while video is already running
						SetKeepScreenOn(true);
					}
					break;

				case nameof(CommunityToolkit::MediaElement.ShowsPlaybackControls):
					avPlayerViewController.ShowsPlaybackControls = Element.ShowsPlaybackControls;
					break;

				case nameof(CommunityToolkit::MediaElement.Source):
					UpdateSource();
					break;

				case nameof(CommunityToolkit::MediaElement.Volume):
					avPlayerViewController.Player.Volume = (float)Element.Volume;
					break;
			}
		}

		void MediaElementSeekRequested(object sender, CommunityToolkit::SeekRequested e)
		{
			if (avPlayerViewController.Player.Status != AVPlayerStatus.ReadyToPlay || avPlayerViewController.Player.CurrentItem == null)
				return;

			var ranges = avPlayerViewController.Player.CurrentItem.SeekableTimeRanges;
			var seekTo = new CMTime(Convert.ToInt64(e.Position.TotalMilliseconds), 1000);
			foreach (var v in ranges)
			{
				if (seekTo >= v.CMTimeRangeValue.Start && seekTo < (v.CMTimeRangeValue.Start + v.CMTimeRangeValue.Duration))
				{
					avPlayerViewController.Player.Seek(seekTo, SeekComplete);
					break;
				}
			}
		}

		void Play()
		{
			var audioSession = AVAudioSession.SharedInstance();
			var err = audioSession.SetCategory(AVAudioSession.CategoryPlayback);
			if (!(err is null))
				Log.Warning("MediaElement", "Failed to set AVAudioSession Category {0}", err.Code);

			audioSession.SetMode(AVAudioSession.ModeMoviePlayback, out err);
			if (!(err is null))
				Log.Warning("MediaElement", "Failed to set AVAudioSession Mode {0}", err.Code);

			err = audioSession.SetActive(true);
			if (!(err is null))
				Log.Warning("MediaElement", "Failed to set AVAudioSession Active {0}", err.Code);

			if (avPlayerViewController.Player != null)
			{
				avPlayerViewController.Player.Play();
				Controller.CurrentState = CommunityToolkit::MediaElementState.Playing;
			}

			if (Element.KeepScreenOn)
			{
				SetKeepScreenOn(true);
			}
		}

		void MediaElementStateRequested(object sender, CommunityToolkit::StateRequested e)
		{
			MediaElementVolumeRequested(this, EventArgs.Empty);

			switch (e.State)
			{
				case CommunityToolkit::MediaElementState.Playing:
					Play();
					break;

				case CommunityToolkit::MediaElementState.Paused:
					if (Element.KeepScreenOn)
					{
						SetKeepScreenOn(false);
					}

					if (avPlayerViewController.Player != null)
					{
						avPlayerViewController.Player.Pause();
						Controller.CurrentState = CommunityToolkit::MediaElementState.Paused;
					}
					break;

				case CommunityToolkit::MediaElementState.Stopped:
					if (Element.KeepScreenOn)
					{
						SetKeepScreenOn(false);
					}

					// iOS has no stop...
					avPlayerViewController?.Player.Pause();
					avPlayerViewController?.Player.Seek(CMTime.Zero);
					Controller.CurrentState = CommunityToolkit::MediaElementState.Stopped;

					var err = AVAudioSession.SharedInstance().SetActive(false);
					if (!(err is null))
						Log.Warning("MediaElement", "Failed to set AVAudioSession Inactive {0}", err.Code);
					break;
			}

			Controller.Position = Position;
		}

		static AVLayerVideoGravity AspectToGravity(Aspect aspect)
		{
			switch (aspect)
			{
				case Aspect.Fill:
					return AVLayerVideoGravity.Resize;
				case Aspect.AspectFit:
				case Aspect.AspectFill:
					return AVLayerVideoGravity.ResizeAspectFill;
				default:
					return AVLayerVideoGravity.ResizeAspect;
			}
		}

		void SeekComplete(bool finished)
		{
			if (finished)
			{
				Controller.OnSeekCompleted();
			}
		}

		void MediaElementVolumeRequested(object sender, EventArgs e) => Controller.Volume = avPlayerViewController.Player.Volume;

		void MediaElementPositionRequested(object sender, EventArgs e) => Controller.Position = Position;

		protected override void OnElementChanged(ElementChangedEventArgs<CommunityToolkit::MediaElement> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				e.OldElement.PropertyChanged -= OnElementPropertyChanged;
				e.OldElement.SeekRequested -= MediaElementSeekRequested;
				e.OldElement.StateRequested -= MediaElementStateRequested;
				e.OldElement.PositionRequested -= MediaElementPositionRequested;
				e.OldElement.VolumeRequested -= MediaElementVolumeRequested;

				if (playedToEndObserver != null)
				{
					NSNotificationCenter.DefaultCenter.RemoveObserver(playedToEndObserver);
					playedToEndObserver = null;
				}

				// stop video if playing
				if (avPlayerViewController?.Player?.CurrentItem != null)
				{
					RemoveStatusObserver();

					avPlayerViewController.Player.Pause();
					avPlayerViewController.Player.Seek(CMTime.Zero);
					avPlayerViewController.Player.ReplaceCurrentItemWithPlayerItem(null);
					AVAudioSession.SharedInstance().SetActive(false);
				}
			}

			if (e.NewElement != null)
			{
				SetNativeControl(avPlayerViewController.View);

				Element.PropertyChanged += OnElementPropertyChanged;
				Element.SeekRequested += MediaElementSeekRequested;
				Element.StateRequested += MediaElementStateRequested;
				Element.PositionRequested += MediaElementPositionRequested;
				Element.VolumeRequested += MediaElementVolumeRequested;

				avPlayerViewController.ShowsPlaybackControls = Element.ShowsPlaybackControls;
				avPlayerViewController.VideoGravity = AspectToGravity(Element.Aspect);
				if (Element.KeepScreenOn)
				{
					SetKeepScreenOn(true);
				}

				playedToEndObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, PlayedToEnd);

				UpdateBackgroundColor();
				UpdateSource();
			}
		}

		void UpdateBackgroundColor() => BackgroundColor = Element.BackgroundColor.ToUIColor();
	}
}