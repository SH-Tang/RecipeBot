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
using AutoFixture;
using FluentAssertions;
using NSubstitute;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Factories;
using RecipeBot.Domain.Models;
using Xunit;

namespace RecipeBot.Domain.Test.Factories;

public class RecipeTagsModelFactoryTest
{
    [Fact]
    public void Creating_model_with_tags_with_invalid_character_length_throws_exception()
    {
        // Setup
        var fixture = new Fixture();
        var tags = fixture.Create<string>();

        int maximumTagsLength = tags.Length - 1;
        var provider = Substitute.For<IRecipeTagModelCharacterLimitProvider>();
        provider.MaximumRecipeTagsLength.Returns(maximumTagsLength);
        var factory = new RecipeTagsModelFactory(provider);

        // Call
        Action call = () => factory.Create(tags);

        // Assert
        var expectedMessage = $"The total tag character length must be less or equal to {maximumTagsLength} characters.";
        call.Should().Throw<ModelCreateException>()
            .WithMessage(expectedMessage);
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData(null)]
    public void Creating_model_with_no_tags_returns_model_with_empty_tag_collection(string tags)
    {
        // Setup
        var provider = Substitute.For<IRecipeTagModelCharacterLimitProvider>();
        provider.MaximumRecipeTagsLength.Returns(int.MaxValue);
        var factory = new RecipeTagsModelFactory(provider);

        // Call
        RecipeTagsModel model = factory.Create(tags);

        // Assert
        model.Tags.Should().BeEmpty();
    }

    [Theory]
    [MemberData(nameof(GetUniqueTagsTestCases))]
    [MemberData(nameof(GetDistinctTagsTestCases))]
    public void Creating_model_with_multiple_tags_returns_model_with_expected_tag_collection(
        string tags, IEnumerable<string> expectedTags)
    {
        // Setup
        var provider = Substitute.For<IRecipeTagModelCharacterLimitProvider>();
        provider.MaximumRecipeTagsLength.Returns(int.MaxValue);
        var factory = new RecipeTagsModelFactory(provider);

        // Call
        RecipeTagsModel model = factory.Create(tags);

        // Assert
        model.Tags.Should().BeEquivalentTo(expectedTags, options => options.WithStrictOrdering());
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