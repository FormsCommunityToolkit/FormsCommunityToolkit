﻿using System;
using Xamarin.Forms;

namespace Xamarin.CommunityToolkit.Core
{
	[Forms.Xaml.TypeConversion(typeof(FileMediaSource))]
	public sealed class FileMediaSourceConverter : TypeConverter
	{
		public override object ConvertFromInvariantString(string value)
			=> value != null
				? (FileMediaSource)MediaSource.FromFile(value)
				: throw new InvalidOperationException($"Cannot convert \"{value}\" into {typeof(FileMediaSource)}");
		}
	}
}
