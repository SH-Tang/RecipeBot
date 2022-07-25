// Copyright (C) 2022 Dennis Tang. All rights reserved.
//
// This file is part of WeekendBot.
//
// WeekendBot is free software: you can redistribute it and/or modify
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

using Discord.Common.Options;
using Xunit;

namespace Discord.Common.Test.Options;

public class StringFormatOptionsTest
{
    [Fact]
    public void SectionKey_Always_ReturnsExpectedValue()
    {
        // Call
        const string key = StringFormatOptions.SectionKey;

        // Assert
        Assert.Equal("StringFormatOptions", key);
    }

    [Fact]
    public void Constructor_Always_ExpectedProperties()
    {
        // Call
        var options = new StringFormatOptions();

        // Assert
        Assert.Equal("F", options.FloatingNumberFormat);
        Assert.Equal("F", options.DateTimeFormat);
        Assert.Equal("c", options.TimeSpanFormat);
    }
}