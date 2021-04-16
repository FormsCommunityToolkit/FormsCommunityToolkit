﻿using System;
using System.Globalization;
using Xamarin.CommunityToolkit.Extensions.Internals;
using Xamarin.Forms;

namespace Xamarin.CommunityToolkit.Converters
{
	/// <summary>
	///     Converts an <see cref="Enum" /> to its underlying <see cref="int" /> value.
	/// </summary>
	public class EnumToIntConverter : ValueConverterExtension, IValueConverter
	{
		/// <summary>
		/// Convert a default <see cref="Enum"/> (i.e., extending <see cref="int"/>) to corresponding underlying <see cref="int"/>
		/// </summary>
		/// <param name="value"><see cref="Enum"/> value to convert</param>
		/// <param name="targetType">The type of the binding target property. This is not used.</param>
		/// <param name="parameter">Additional parameter for converter. This is not used.</param>
		/// <param name="culture">The culture to use in the converter. This is not used.</param>
		/// <returns>The underlying <see cref="int"/> value of the passed value, or -1 if the value is not an enumeration type</returns>
		public object Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture) =>
			value is Enum ? System.Convert.ToInt32(value) : throw new ArgumentException($"{value?.GetType().Name} is not a valid enumeration type");

		/// <summary>
		/// Returns the <see cref="Enum"/> associated with the specified <see cref="int"/> value defined in the targetType
		/// </summary>
		/// <param name="value"><see cref="Enum"/> value to convert</param>
		/// <param name="targetType">The type of the binding target property. Expected to be an enum.</param>
		/// <param name="parameter">Additional parameter for converter. This is not used.</param>
		/// <param name="culture">The culture to use in the converter. This is not used.</param>
		/// <returns>The underlying <see cref="Enum"/> of the associated targetType, or 0 if the value is not an enumeration type</returns>
		/// <exception cref="ArgumentException">If value is not a valid value in the targetType enum</exception>
		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture) =>
			value is int enumIntVal && Enum.IsDefined(targetType, enumIntVal) ? Enum.ToObject(targetType, enumIntVal) : throw new ArgumentException($"{value} is not valid for {targetType.Name}");
	}
}
