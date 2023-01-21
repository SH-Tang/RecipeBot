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

using Discord.Common.Options;
using FluentAssertions;
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
        key.Should().Be("StringFormatOptions");
    }

    [Fact]
    public void Constructor_Always_ExpectedProperties()
    {
        // Call
        var options = new StringFormatOptions();

        // Assert
        options.FloatingNumberFormat.Should().Be("F");
        options.DateTimeFormat.Should().Be("F");
        options.TimeSpanFormat.Should().Be("c");
    }
}