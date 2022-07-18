using System;
using System.Collections.Generic;
using System.Globalization;
using WeekendBot.Core.Options;
using Xunit;

namespace WeekendBot.Components.Test;

public class StringFormatExtensionsTest
{
    public StringFormatExtensionsTest()
    {
        CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("nl-NL");
    }

    [Theory]
    [InlineData("F2", 3.14159265359, "3,14")]
    [InlineData("F4", 3.14159265359, "3,1416")]
    [InlineData("E2", 3.14159265359, "3,14E+000")]
    public void FormatDouble_WithArguments_ReturnsExpectedString(string format, double value, string expectedValue)
    {
        // Setup
        var options = new StringFormatOptions
        {
            FloatingNumberFormat = format
        };

        // Call
        string formattedValue = options.Format(value);

        // Assert
        Assert.Equal(expectedValue, formattedValue);
    }

    [Theory]
    [MemberData(nameof(GetDateTimeTestCaseData))]
    public void FormatDateTime_WithArguments_ReturnsExpectedString(string format, DateTime value, string expectedValue)
    {
        // Setup
        var options = new StringFormatOptions
        {
            DateTimeFormat = format
        };

        // Call
        string formattedValue = options.Format(value);

        // Assert
        Assert.Equal(expectedValue, formattedValue);
    }

    [Theory]
    [MemberData(nameof(GetTimeSpanTestCaseData))]
    public void FormatTimeSpan_WithArguments_ReturnsExpectedString(string format, TimeSpan value, string expectedValue)
    {
        // Setup
        var options = new StringFormatOptions
        {
            TimeSpanFormat = format
        };

        // Call
        string formattedValue = options.Format(value);

        // Assert
        Assert.Equal(expectedValue, formattedValue);
    }

    private static IEnumerable<object[]> GetDateTimeTestCaseData()
    {
        var dateTime = new DateTime(2022, 2, 14, 14, 20, 30);
        yield return new object[]
        {
            "D",
            dateTime,
            "maandag 14 februari 2022"
        };

        yield return new object[]
        {
            "F",
            dateTime,
            "maandag 14 februari 2022 14:20:30"
        };

        yield return new object[]
        {
            "dddd d\\/MM\\/yy HH:mm:ss",
            dateTime,
            "maandag 14/02/22 14:20:30"
        };
    }

    private static IEnumerable<object[]> GetTimeSpanTestCaseData()
    {
        var timeSpan = new TimeSpan(20, 2, 30, 59, 121);
        yield return new object[]
        {
            "c",
            timeSpan,
            "20.02:30:59.1210000"
        };

        yield return new object[]
        {
            "d\\.hh\\:mm\\:ss",
            timeSpan,
            "20.02:30:59"
        };
    }
}