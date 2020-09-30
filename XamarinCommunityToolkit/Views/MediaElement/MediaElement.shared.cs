﻿using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace Xamarin.CommunityToolkit.UI.Views
{
	public sealed class MediaElement : View, IMediaElementController
	{
		public static readonly BindableProperty AspectProperty =
		  BindableProperty.Create(nameof(Aspect), typeof(Aspect), typeof(MediaElement), Aspect.AspectFit);

		public static readonly BindableProperty AutoPlayProperty =
		  BindableProperty.Create(nameof(AutoPlay), typeof(bool), typeof(MediaElement), true);

		public static readonly BindableProperty BufferingProgressProperty =
		  BindableProperty.Create(nameof(BufferingProgress), typeof(double), typeof(MediaElement), 0.0);

		public static readonly BindableProperty CurrentStateProperty =
		  BindableProperty.Create(nameof(CurrentState), typeof(MediaElementState), typeof(MediaElement), MediaElementState.Closed);

		public static readonly BindableProperty DurationProperty =
		  BindableProperty.Create(nameof(Duration), typeof(TimeSpan?), typeof(MediaElement), null);

		public static readonly BindableProperty IsLoopingProperty =
		  BindableProperty.Create(nameof(IsLooping), typeof(bool), typeof(MediaElement), false);

		public static readonly BindableProperty KeepScreenOnProperty =
		  BindableProperty.Create(nameof(KeepScreenOn), typeof(bool), typeof(MediaElement), false);

		public static readonly BindableProperty PositionProperty =
		  BindableProperty.Create(nameof(Position), typeof(TimeSpan), typeof(MediaElement), TimeSpan.Zero);

		public static readonly BindableProperty ShowsPlaybackControlsProperty =
		  BindableProperty.Create(nameof(ShowsPlaybackControls), typeof(bool), typeof(MediaElement), false);

		public static readonly BindableProperty SourceProperty =
		  BindableProperty.Create(nameof(Source), typeof(MediaSource), typeof(MediaElement),
			  propertyChanging: OnSourcePropertyChanging, propertyChanged: OnSourcePropertyChanged);

		public static readonly BindableProperty VideoHeightProperty =
		  BindableProperty.Create(nameof(VideoHeight), typeof(int), typeof(MediaElement));

		public static readonly BindableProperty VideoWidthProperty =
		  BindableProperty.Create(nameof(VideoWidth), typeof(int), typeof(MediaElement));

		public static readonly BindableProperty VolumeProperty =
		  BindableProperty.Create(nameof(Volume), typeof(double), typeof(MediaElement), 1.0, BindingMode.TwoWay, new BindableProperty.ValidateValueDelegate(ValidateVolume));

		static bool ValidateVolume(BindableObject o, object newValue)
		{
			var d = (double)newValue;

			return d >= 0.0 && d <= 1.0;
		}

		public Aspect Aspect
		{
			get => (Aspect)GetValue(AspectProperty);
			set => SetValue(AspectProperty, value);
		}

		public bool AutoPlay
		{
			get => (bool)GetValue(AutoPlayProperty);
			set => SetValue(AutoPlayProperty, value);
		}

		public double BufferingProgress => (double)GetValue(BufferingProgressProperty);

		public bool CanSeek => Source != null && Duration.HasValue;

		public MediaElementState CurrentState => (MediaElementState)GetValue(CurrentStateProperty);

		public TimeSpan? Duration => (TimeSpan?)GetValue(DurationProperty);

		public bool IsLooping
		{
			get => (bool)GetValue(IsLoopingProperty);
			set => SetValue(IsLoopingProperty, value);
		}

		public bool KeepScreenOn
		{
			get => (bool)GetValue(KeepScreenOnProperty);
			set => SetValue(KeepScreenOnProperty, value);
		}

		public bool ShowsPlaybackControls
		{
			get => (bool)GetValue(ShowsPlaybackControlsProperty);
			set => SetValue(ShowsPlaybackControlsProperty, value);
		}

		public TimeSpan Position
		{
			get
			{
				PositionRequested?.Invoke(this, EventArgs.Empty);
				return (TimeSpan)GetValue(PositionProperty);
			}

			set => SeekRequested?.Invoke(this, new SeekRequested(value));
		}

		[Forms.TypeConverter(typeof(MediaSourceConverter))]
		public MediaSource Source
		{
			get => (MediaSource)GetValue(SourceProperty);
			set => SetValue(SourceProperty, value);
		}

		public int VideoHeight => (int)GetValue(VideoHeightProperty);

		public int VideoWidth => (int)GetValue(VideoWidthProperty);

		public double Volume
		{
			get
			{
				VolumeRequested?.Invoke(this, EventArgs.Empty);
				return (double)GetValue(VolumeProperty);
			}
			set => SetValue(VolumeProperty, value);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler<SeekRequested> SeekRequested;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler<StateRequested> StateRequested;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler PositionRequested;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler VolumeRequested;

		public event EventHandler MediaEnded;

		public event EventHandler MediaFailed;

		public event EventHandler MediaOpened;

		public event EventHandler SeekCompleted;

		public void Play() => StateRequested?.Invoke(this, new StateRequested(MediaElementState.Playing));

		public void Pause() => StateRequested?.Invoke(this, new StateRequested(MediaElementState.Paused));

		public void Stop() => StateRequested?.Invoke(this, new StateRequested(MediaElementState.Stopped));

		double IMediaElementController.BufferingProgress { get => (double)GetValue(BufferingProgressProperty); set => SetValue(BufferingProgressProperty, value); }

		MediaElementState IMediaElementController.CurrentState { get => (MediaElementState)GetValue(CurrentStateProperty); set => SetValue(CurrentStateProperty, value); }

		TimeSpan? IMediaElementController.Duration { get => (TimeSpan?)GetValue(DurationProperty); set => SetValue(DurationProperty, value); }

		TimeSpan IMediaElementController.Position { get => (TimeSpan)GetValue(PositionProperty); set => SetValue(PositionProperty, value); }

		int IMediaElementController.VideoHeight { get => (int)GetValue(VideoHeightProperty); set => SetValue(VideoHeightProperty, value); }

		int IMediaElementController.VideoWidth { get => (int)GetValue(VideoWidthProperty); set => SetValue(VideoWidthProperty, value); }

		double IMediaElementController.Volume
		{
			get => (double)GetValue(VolumeProperty);
			set => SetValue(VolumeProperty, value);
		}

		void IMediaElementController.OnMediaEnded()
		{
			SetValue(CurrentStateProperty, MediaElementState.Stopped);
			MediaEnded?.Invoke(this, EventArgs.Empty);
		}

		void IMediaElementController.OnMediaFailed() => MediaFailed?.Invoke(this, EventArgs.Empty);

		void IMediaElementController.OnMediaOpened() => MediaOpened?.Invoke(this, EventArgs.Empty);

		void IMediaElementController.OnSeekCompleted() => SeekCompleted?.Invoke(this, EventArgs.Empty);

		protected override void OnBindingContextChanged()
		{
			if (Source != null)
				SetInheritedBindingContext(Source, BindingContext);

			base.OnBindingContextChanged();
		}

		void OnSourceChanged(object sender, EventArgs eventArgs)
		{
			OnPropertyChanged(SourceProperty.PropertyName);
			InvalidateMeasure();
		}

		static void OnSourcePropertyChanged(BindableObject bindable, object oldvalue, object newvalue) =>
			((MediaElement)bindable).OnSourcePropertyChanged((MediaSource)oldvalue, (MediaSource)newvalue);

		void OnSourcePropertyChanged(MediaSource oldvalue, MediaSource newvalue)
		{
			if (newvalue != null)
			{
				newvalue.SourceChanged += OnSourceChanged;
				SetInheritedBindingContext(newvalue, BindingContext);
			}

			InvalidateMeasure();
		}

		static void OnSourcePropertyChanging(BindableObject bindable, object oldvalue, object newvalue) =>
			((MediaElement)bindable).OnSourcePropertyChanging((MediaSource)oldvalue, (MediaSource)newvalue);

		void OnSourcePropertyChanging(MediaSource oldvalue, MediaSource newvalue)
		{
			if (oldvalue == null)
				return;

			oldvalue.SourceChanged -= OnSourceChanged;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public class SeekRequested : EventArgs
	{
		public TimeSpan Position { get; }

		public SeekRequested(TimeSpan position) => Position = position;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public class StateRequested : EventArgs
	{
		public MediaElementState State { get; }

		public StateRequested(MediaElementState state) => State = state;
	}

	public interface IMediaElementController
	{
		double BufferingProgress { get; set; }

		MediaElementState CurrentState { get; set; }

		TimeSpan? Duration { get; set; }

		TimeSpan Position { get; set; }

		int VideoHeight { get; set; }

		int VideoWidth { get; set; }

		double Volume { get; set; }

		void OnMediaEnded();

		void OnMediaFailed();

		void OnMediaOpened();

		void OnSeekCompleted();
	}
}