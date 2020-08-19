﻿using System;
using System.Globalization;
using Xamarin.CommunityToolkit.Converters;
using Xunit;

namespace Xamarin.CommunityToolkit.UnitTests.Converters
{
	public class BoolToObjectConverter_Tests
	{
		public const string TrueTestObject = nameof(TrueTestObject);
		public const string FalseTestObject = nameof(FalseTestObject);

		[Theory]
		[InlineData(true, TrueTestObject, FalseTestObject, TrueTestObject)]
		[InlineData(false, TrueTestObject, FalseTestObject, FalseTestObject)]
		public void BoolToObjectConvert(bool value, object trueObject, object falseObject, object expectedResult)
		{
			var boolObjectConverter = new BoolToObjectConverter();
			boolObjectConverter.TrueObject = trueObject;
			boolObjectConverter.FalseObject = falseObject;

			var result = boolObjectConverter.Convert(value, typeof(BoolToObjectConverter_Tests), null, CultureInfo.CurrentCulture);

			Assert.Equal(result, expectedResult);
		}

		[Theory]
		[InlineData(TrueTestObject, TrueTestObject, FalseTestObject, true)]
		[InlineData(FalseTestObject, TrueTestObject, FalseTestObject, false)]
		public void BoolToObjectConvertBack(object value, object trueObject, object falseObject, bool expectedResult)
		{
			var boolObjectConverter = new BoolToObjectConverter();
			boolObjectConverter.TrueObject = trueObject;
			boolObjectConverter.FalseObject = falseObject;

			var result = boolObjectConverter.ConvertBack(value, typeof(BoolToObjectConverter_Tests), null, CultureInfo.CurrentCulture);

			Assert.Equal(result, expectedResult);
		}

		[Theory]
		[InlineData("")]
		public void BoolToObjectInValidValuesThrowArgumenException(object value)
		{
			var boolObjectConverter = new BoolToObjectConverter();
			Assert.Throws<ArgumentException>(() => boolObjectConverter.Convert(value, typeof(BoolToObjectConverter_Tests), null, CultureInfo.CurrentCulture));
		}
	}
}