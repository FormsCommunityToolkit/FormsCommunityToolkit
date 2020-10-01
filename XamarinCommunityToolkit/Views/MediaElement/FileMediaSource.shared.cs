﻿using Xamarin.Forms;

namespace Xamarin.CommunityToolkit.UI.Views
{
	[TypeConverter(typeof(FileMediaSourceConverter))]
	public sealed class FileMediaSource : MediaSource
	{
		public static readonly BindableProperty FileProperty = BindableProperty.Create(nameof(File), typeof(string), typeof(FileMediaSource), default(string));

		public string File
		{
			get => (string)GetValue(FileProperty);
			set => SetValue(FileProperty, value);
		}

		public override string ToString() => $"File: {File}";

		public static implicit operator FileMediaSource(string file)
		{
			return (FileMediaSource)FromFile(file);
		}

		public static implicit operator string(FileMediaSource file) => file?.File;

		protected override void OnPropertyChanged(string propertyName = null)
		{
			if (propertyName == FileProperty.PropertyName)
				OnSourceChanged();

			base.OnPropertyChanged(propertyName);
		}
	}
}