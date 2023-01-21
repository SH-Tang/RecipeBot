// Copyright (C) 2022 Dennis Tang. All rights reserved.
//
// This file is part of RecipeBot.
//
// RecipeBot is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Globalization;
using Discord.Common.Options;
using FluentAssertions;
using Xunit;

namespace Discord.Common.Test.Options;

public class StringFormatOptionsExtensionsTest
{
    public StringFormatOptionsExtensionsTest()
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
        formattedValue.Should().Be(expectedValue);
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
        formattedValue.Should().Be(expectedValue);
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
        formattedValue.Should().Be(expectedValue);
    }

    public static IEnumerable<object[]> GetDateTimeTestCaseData()
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

    public static IEnumerable<object[]> GetTimeSpanTestCaseData()
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