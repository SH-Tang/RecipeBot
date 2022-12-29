﻿// Copyright (C) 2022 Dennis Tang. All rights reserved.
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
using NSubstitute;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Factories;
using RecipeBot.Domain.Models;
using Xunit;

namespace RecipeBot.Domain.Test.Factories;

public class RecipeTagsModelFactoryTest
{
    [Fact]
    public void Creating_model_with_invalid_category_throws_exception()
    {
        // Setup
        const RecipeCategory invalidCategory = (RecipeCategory)(-1);

        var fixture = new Fixture();
        var tags = fixture.Create<string>();

        var provider = Substitute.For<IRecipeTagModelCharacterLimitProvider>();
        var factory = new RecipeTagsModelFactory(provider);

        // Call
        Action call = () => factory.Create(tags);

        // Assert
        Assert.Throws<InvalidEnumArgumentException>(call);
    }

    [Fact]
    public void Creating_model_with_tags_with_invalid_character_length_throws_exception()
    {
        // Setup
        var fixture = new Fixture();
        var category = fixture.Create<RecipeCategory>();
        var tags = fixture.Create<string>();

        int maximumTagsLength = tags.Length - 1;
        var provider = Substitute.For<IRecipeTagModelCharacterLimitProvider>();
        provider.MaximumRecipeTagsLength.Returns(maximumTagsLength);
        var factory = new RecipeTagsModelFactory(provider);

        // Call
        Action call = () => factory.Create(tags);

        // Assert
        var exception = Assert.Throws<ModelCreateException>(call);
        string expectedMessage = $"The total tag character length must be less or equal to {maximumTagsLength} characters.";
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData(null)]
    public void Creating_model_with_valid_category_and_no_tags_returns_model_containing_only_category_tag(string tags)
    {
        // Setup
        var provider = Substitute.For<IRecipeTagModelCharacterLimitProvider>();
        provider.MaximumRecipeTagsLength.Returns(int.MaxValue);
        var factory = new RecipeTagsModelFactory(provider);

        // Call
        RecipeTagsModel model = factory.Create(tags);

        // Assert
        Assert.Empty(model.Tags);
    }

    [Theory]
    [MemberData(nameof(GetUniqueTagsTestCases))]
    [MemberData(nameof(GetDistinctTagsTestCases))]
    public void Creating_model_with_valid_category_and_multiple_tags_returns_expected_tag_model(
        string tags, IEnumerable<string> expectedTags)
    {
        // Setup
        var provider = Substitute.For<IRecipeTagModelCharacterLimitProvider>();
        provider.MaximumRecipeTagsLength.Returns(int.MaxValue);
        var factory = new RecipeTagsModelFactory(provider);

        // Call
        RecipeTagsModel model = factory.Create(tags);

        // Assert
        Assert.Equal(expectedTags, model.Tags);
    }

    public static IEnumerable<object[]> GetUniqueTagsTestCases()
    {
        var expectedTagCollection = new[]
        {
            "tag1",
            "tag2",
            "tag3"
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
            "tag1",
            "tag2"
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
            "Tag1, Tag2,     Tag1",
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

        yield return new object[]
        {
            "Tag1, Tag2, tag1",
            expectedTagCollection
        };

        yield return new object[]
        {
            "Tag1, Tag2, TAG1",
            expectedTagCollection
        };

        yield return new object[]
        {
            "Tag1, Tag2, Tag     1",
            expectedTagCollection
        };
    }
}