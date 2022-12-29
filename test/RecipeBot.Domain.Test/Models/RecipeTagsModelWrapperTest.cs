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
using RecipeBot.Domain.Models;
using RecipeBot.Domain.TestUtils;
using Xunit;

namespace RecipeBot.Domain.Test.Models;

public class RecipeTagsModelWrapperTest
{
    [Fact]
    public void Wrapper_with_invalid_category_throws_exception()
    {
        // Setup
        const RecipeCategory invalidCategory = (RecipeCategory)(-1);

        var fixture = new Fixture();
        IEnumerable<string> tags = fixture.CreateMany<string>();
        var tagsModel = new RecipeTagsModel(tags);

        // Call
        Action call = () => new RecipeTagsModelWrapper(tagsModel, invalidCategory);

        // Assert
        Assert.Throws<InvalidEnumArgumentException>(call);
    }

    [Fact]
    public void Wrapper_with_model_and_valid_category_returns_model_tags()
    {
        // Setup
        var tagsModel = new RecipeTagsModel(Enumerable.Empty<string>());

        var fixture = new Fixture();
        var category = fixture.Create<RecipeCategory>();

        // Call
        var wrapper = new RecipeTagsModelWrapper(tagsModel, category);

        // Assert
        Assert.Same(tagsModel.Tags, wrapper.Tags);
    }

    [Theory]
    [InlineData(RecipeCategory.Dessert)]
    [InlineData(RecipeCategory.Fish)]
    [InlineData(RecipeCategory.Meat)]
    [InlineData(RecipeCategory.Pastry)]
    [InlineData(RecipeCategory.Snack)]
    [InlineData(RecipeCategory.Vegan)]
    [InlineData(RecipeCategory.Vegetarian)]
    [InlineData(RecipeCategory.Drinks)]
    [InlineData(RecipeCategory.Other)]
    public void Wrapper_without_tags_and_valid_category_returns_expected_string_representation(
        RecipeCategory category)
    {
        // Setup
        var tagsModel = new RecipeTagsModel(Enumerable.Empty<string>());
        var wrapper = new RecipeTagsModelWrapper(tagsModel, category);

        // Call
        var stringRepresentation = wrapper.ToString();

        // Assert
        string expectedFirstTag = TagTestHelper.CategoryMapping[category];
        Assert.Equal(expectedFirstTag, stringRepresentation);
    }

    [Fact]
    public void Wrapper_with_tags_and_valid_category_returns_expected_string_representation()
    {
        // Setup
        var fixture = new Fixture();
        var category = fixture.Create<RecipeCategory>();
        IEnumerable<string>? tags = fixture.CreateMany<string>();

        var tagsModel = new RecipeTagsModel(tags);
        var wrapper = new RecipeTagsModelWrapper(tagsModel, category);

        // Call
        var stringRepresentation = wrapper.ToString();

        // Assert
        string expectedFirstTag = TagTestHelper.CategoryMapping[category];
        var expectedStringRepresentation = $"{expectedFirstTag}, {string.Join(", ", tags)}";
        Assert.Equal(expectedStringRepresentation, stringRepresentation);
    }

    [Theory]
    [MemberData(nameof(GetTagsLengthTestCases))]
    public void Wrapper_with_tags_and_valid_category_returns_expected_total_length(
        IEnumerable<string> tags)
    {
        // Setup
        var fixture = new Fixture();
        var category = fixture.Create<RecipeCategory>();

        var tagsModel = new RecipeTagsModel(tags);
        var wrapper = new RecipeTagsModelWrapper(tagsModel, category);

        // Call
        int length = wrapper.TotalLength;

        // Assert
        Assert.Equal(TagTestHelper.GetTotalTagsLength(category, tagsModel), length);
    }

    public static IEnumerable<object[]> GetTagsLengthTestCases()
    {
        yield return new object[]
        {
            Enumerable.Empty<string>()
        };

        yield return new object[]
        {
            new[]
            {
                "Tag 1",
                "Tag 2",
                "Tag 3"
            }
        };
    }
}