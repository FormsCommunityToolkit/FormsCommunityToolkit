﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Toolkit.Xamarin.Forms.Converters;
using Xunit;

namespace Microsoft.Toolkit.Xamarin.Forms.UnitTests.Converters
{
    public class ListIsNotNullOrEmptyConverter_Tests
    {
        public static IEnumerable<object[]> GetData()
        {
            return new List<object[]>
            {
                new object[] { new List<string>(), false},
                new object[] { new List<string>() { "TestValue"}, true},
                new object[] { null, false},
                new object[] { Enumerable.Range(1, 3), true},
            };
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void ListIsNotNullOrEmptyConverter(object value, bool expectedResult)
        {
            var listIsNotNullOrEmptyConverter = new ListIsNotNullOrEmptyConverter();

            var result = listIsNotNullOrEmptyConverter.Convert(value, typeof(ListIsNotNullOrEmptyConverter), null, CultureInfo.CurrentCulture);

            Assert.Equal(result, expectedResult);
        }
       
        [Theory]
        [InlineData(0)]
        public void InValidConverterValuesThrowArgumenException(object value)
        {
            var listIsNotNullOrEmptyConverter = new ListIsNotNullOrEmptyConverter();

            Assert.Throws<ArgumentException>(() => listIsNotNullOrEmptyConverter.Convert(value, typeof(ListIsNotNullOrEmptyConverter), null, CultureInfo.CurrentCulture));
        }
    }
}
