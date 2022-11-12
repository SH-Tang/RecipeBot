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

using System.Collections.Generic;
using System.Linq;
using RecipeBot.Domain.Models;
using Xunit;

namespace RecipeBot.Domain.Test.Models;

public class RecipeTagsModelTest
{
    [Fact]
    public void Model_with_tags_returns_expected_string_representation()
    {
        // Setup
        var tags = new[]
        {
            "Tag 1",
            "Tag 2",
            "Tag 3"
        };

        var model = new RecipeTagsModel(tags);

        // Call
        var stringRepresentation = model.ToString();

        // Assert
        Assert.Equal("Tag 1, Tag 2, Tag 3", stringRepresentation);
    }

    [Theory]
    [MemberData(nameof(GetTagsLengthTestCases))]
    public void Model_with_tags_returns_expected_total_length(
        IEnumerable<string> tags, int expectedLength)
    {
        // Setup
        var model = new RecipeTagsModel(tags);

        // Call
        int length = model.TotalLength;

        // Assert
        Assert.Equal(expectedLength, length);
    }

    public static IEnumerable<object[]> GetTagsLengthTestCases()
    {
        yield return new object[]
        {
            Enumerable.Empty<string>(),
            0
        };

        yield return new object[]
        {
            new[]
            {
                "Tag 1",
                "Tag 2",
                "Tag 3"
            },
            "Tag 1, Tag 2, Tag 3".Length
        };
    }
}