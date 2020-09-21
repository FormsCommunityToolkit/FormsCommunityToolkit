﻿using System;
using System.Globalization;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;

namespace Xamarin.CommunityToolkit.Converters
{
	/// <summary>
	/// Converts the incoming value to a boolean indicating whether or not the value is null or empty.
	/// </summary>
	public class IsNullOrEmptyConverter : ValueConverterExtension, IValueConverter
	{
		/// <summary>
		/// Converts the incoming value to a boolean indicating whether or not the value is null or empty.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">Additional parameter for the converter to handle. Not implemented.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>A boolean indicating if the incoming value is null or empty.</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
			value switch
			{
				null => true,
				string x => string.IsNullOrWhiteSpace(x),
				_ => false,
			};

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}