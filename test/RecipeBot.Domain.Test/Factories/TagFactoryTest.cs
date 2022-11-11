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
using System.ComponentModel;
using System.Linq;
using AutoFixture;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Factories;
using Xunit;

namespace RecipeBot.Domain.Test.Factories;

public class TagFactoryTest
{
    [Fact]
    public void Create_tags_with_invalid_category_throws_exception()
    {
        // Setup
        var fixture = new Fixture();
        var secondTagArgument = fixture.Create<string>();

        const RecipeCategory invalidCategory = (RecipeCategory)(-1);

        // Call
        Action call = () => TagFactory.Create(invalidCategory, secondTagArgument);


        // Assert
        Assert.Throws<InvalidEnumArgumentException>(call);
    }

    [Theory]
    [InlineData(RecipeCategory.Dessert, "Dessert")]
    [InlineData(RecipeCategory.Fish, "Fish")]
    [InlineData(RecipeCategory.Meat, "Meat")]
    [InlineData(RecipeCategory.Pastry, "Pastry")]
    [InlineData(RecipeCategory.Snack, "Snack")]
    [InlineData(RecipeCategory.Vegan, "Vegan")]
    [InlineData(RecipeCategory.Vegetarian, "Vegetarian")]
    [InlineData(RecipeCategory.Drinks, "Drinks")]
    [InlineData(RecipeCategory.Other, "Other")]
    public void Create_tags_with_valid_category_and_tags_returns_expected_tags(
        RecipeCategory category, string expectedFirstTag)
    {
        // Setup
        var fixture = new Fixture();
        var secondTagArgument = fixture.Create<string>();

        // Call
        IEnumerable<string> createdTags = TagFactory.Create(category, secondTagArgument);

        // Assert
        Assert.Equal(2, createdTags.Count());
        Assert.Collection(createdTags,
                          firstTag =>
                          {
                              Assert.Equal(expectedFirstTag, firstTag);
                          },
                          secondTag =>
                          {
                              Assert.Equal(secondTagArgument, secondTag);
                          });
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData(null)]
    public void Create_tags_with_valid_category_and_no_tags_returns_only_category_tag(string tags)
    {
        // Setup
        const RecipeCategory category = RecipeCategory.Other;

        // Call
        IEnumerable<string> createdTags = TagFactory.Create(category, tags);

        // Assert
        Assert.Single(createdTags);
        Assert.Collection(createdTags,
                          firstTag =>
                          {
                              Assert.Equal("Other", firstTag);
                          });
    }

    [Theory]
    [MemberData(nameof(GetUniqueTagsTestCases))]
    [MemberData(nameof(GetDistinctTagsTestCases))]
    public void Create_tags_with_valid_category_and_multiple_tags_returns_expected_tags(
        string tags, IEnumerable<string> expectedCreatedTags)
    {
        // Setup
        const RecipeCategory category = RecipeCategory.Other;

        // Call
        IEnumerable<string> createdTags = TagFactory.Create(category, tags);

        // Assert
        var expectedTags = new List<string>
        {
            "Other"
        };
        expectedTags.AddRange(expectedCreatedTags);

        Assert.Equal(expectedTags, createdTags);
    }

    public static IEnumerable<object[]> GetUniqueTagsTestCases()
    {
        var expectedTagCollection = new[]
        {
            "Tag1",
            "Tag2",
            "Tag3"
        };
        yield return new object[]
        {
            "Tag1, Tag2, Tag3",
            expectedTagCollection
        };

        yield return new object[]
        {
            "Tag1, Tag2,Tag3",
            expectedTagCollection
        };
        yield return new object[]
        {
            "Tag1, Tag2     , Tag3",
            expectedTagCollection
        };

        yield return new object[]
        {
            "Tag1, Tag2, Tag3     ",
            expectedTagCollection
        };

        yield return new object[]
        {
            "Tag1      , Tag2, Tag3",
            expectedTagCollection
        };

        yield return new object[]
        {
            "       Tag1, Tag2, Tag3",
            expectedTagCollection
        };
    }

    public static IEnumerable<object[]> GetDistinctTagsTestCases()
    {
        var expectedTagCollection = new[]
        {
            "Tag1",
            "Tag2"
        };
        yield return new object[]
        {
            "Tag1, Tag2, Tag1",
            expectedTagCollection
        };

        yield return new object[]
        {
            "Tag1, Tag2,Tag1",
            expectedTagCollection
        };
        yield return new object[]
        {
            "Tag1, Tag2     , Tag1",
            expectedTagCollection
        };

        yield return new object[]
        {
            "Tag1, Tag2, Tag1     ",
            expectedTagCollection
        };

        yield return new object[]
        {
            "Tag1      , Tag2, Tag1",
            expectedTagCollection
        };

        yield return new object[]
        {
            "       Tag1, Tag2, Tag1",
            expectedTagCollection
        };
    }
}