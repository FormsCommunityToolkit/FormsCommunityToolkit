﻿using System;
using System.Globalization;
using Xamarin.CommunityToolkit.Extensions.Internals;
using Xamarin.Forms;

namespace Xamarin.CommunityToolkit.Converters
{
	public class IsBetweenConverter : ValueConverterExtension, IValueConverter
	{
		public object? MinValue { get; set; }

		public object? MaxValue { get; set; }

		public object Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
		{
			if (value == null)
				throw new ArgumentException("Value cannot be null", nameof(value));

			if (MinValue == null)
				throw new ArgumentException("Value cannot be null", nameof(MinValue));

			if (MaxValue == null)
				throw new ArgumentException("Value cannot be null", nameof(MaxValue));

			if (value.GetType() != MinValue.GetType())
				throw new ArgumentException("Values cannot be different types", nameof(MinValue));

			if (value.GetType() != MaxValue.GetType())
				throw new ArgumentException("Values cannot be different types", nameof(MinValue));

			if (value is IComparable comparable)
			{
				return comparable.CompareTo(MinValue) >= 0 && comparable.CompareTo(MaxValue) <= 0;
			}

			return false;
		}

		public object ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture) =>
			throw new NotImplementedException();
	}
}
