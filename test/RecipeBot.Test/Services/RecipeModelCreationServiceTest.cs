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
using AutoFixture;
using Discord;
using FluentAssertions;
using NSubstitute;
using RecipeBot.Discord.Data;
using RecipeBot.Discord.Views;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Factories;
using RecipeBot.Domain.Models;
using RecipeBot.Services;
using RecipeBot.TestUtils;
using Xunit;

namespace RecipeBot.Test.Services;

public class RecipeModelCreationServiceTest
{
    [Fact]
    public void Recipe_with_valid_data_and_invalid_category_throws_exception()
    {
        // Setup
        var fixture = new Fixture();

        const DiscordRecipeCategory category = (DiscordRecipeCategory)(-1);

        var user = Substitute.For<IUser>();
        user.Id.Returns(fixture.Create<ulong>());

        var modal = new RecipeModal
        {
            RecipeTitle = "Recipe title",
            Ingredients = "My ingredients",
            CookingSteps = "My recipe steps",
            Notes = "My notes",
            Tags = "Tag1, Tag2, Tag1"
        };

        IRecipeModelCharacterLimitProvider limitProvider = CreateDiscordCharacterLimitProvider();

        var service = new RecipeModelCreationService(limitProvider);

        // Call
        Action call = () => service.CreateRecipeModel(modal, user, category);

        // Assert
        call.Should().Throw<InvalidEnumArgumentException>();
    }

    [Fact]
    public void Recipe_with_invalid_data_and_valid_category_throws_exception()
    {
        // Setup
        var fixture = new Fixture();
        var category = fixture.Create<DiscordRecipeCategory>();

        var user = Substitute.For<IUser>();
        user.Id.Returns(fixture.Create<ulong>());

        var modal = new RecipeModal
        {
            RecipeTitle = "Recipe title",
            Ingredients = "My ingredients",
            CookingSteps = "My recipe steps",
            Notes = "My notes",
            Tags = "Tag1, Tag2, Tag1"
        };

        var limitProvider = Substitute.For<IRecipeModelCharacterLimitProvider>();
        limitProvider.MaximumTitleLength.Returns(EmbedBuilder.MaxTitleLength);
        limitProvider.MaximumFieldNameLength.Returns(EmbedFieldBuilder.MaxFieldNameLength);
        limitProvider.MaximumFieldDataLength.Returns(EmbedFieldBuilder.MaxFieldValueLength);
        limitProvider.MaximumRecipeTagsLength.Returns(EmbedFooterBuilder.MaxFooterTextLength);

        var service = new RecipeModelCreationService(limitProvider);

        // Call
        Action call = () => service.CreateRecipeModel(modal, user, category);

        // Assert
        call.Should().Throw<ModelCreateException>();
    }

    [Theory]
    [MemberData(nameof(GetRecipeCategories))]
    public void Recipe_with_valid_data_returns_expected_model(DiscordRecipeCategory category)
    {
        // Setup
        var fixture = new Fixture();
        var user = Substitute.For<IUser>();
        user.Id.Returns(fixture.Create<ulong>());

        const string recipeTitle = "Recipe title";
        const string recipeIngredients = "My ingredients";
        const string recipeSteps = "My recipe steps";
        const string recipeNotes = "My notes";
        const string tags = "Tag1, TAG1, tag1, tag    1,      tag1, tag1      , tag2";
        var modal = new RecipeModal
        {
            RecipeTitle = recipeTitle,
            Ingredients = recipeIngredients,
            CookingSteps = recipeSteps,
            Notes = recipeNotes,
            Tags = tags
        };

        IRecipeModelCharacterLimitProvider limitProvider = CreateDiscordCharacterLimitProvider();
        var service = new RecipeModelCreationService(limitProvider);

        // Call
        RecipeModel model = service.CreateRecipeModel(modal, user, category);

        // Assert
        RecipeModelTestHelper.AssertFullRecipeProperties(user, category, modal, model);
    }

    public static IEnumerable<object[]> GetRecipeCategories()
    {
        yield return new object[]
        {
            DiscordRecipeCategory.Meat
        };

        yield return new object[]
        {
            DiscordRecipeCategory.Fish
        };

        yield return new object[]
        {
            DiscordRecipeCategory.Vegetarian
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Vegan
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Drinks
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Pastry
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Dessert
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Snack
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Other
        };
    }

    private static IRecipeModelCharacterLimitProvider CreateDiscordCharacterLimitProvider()
    {
        var limitProvider = Substitute.For<IRecipeModelCharacterLimitProvider>();
        limitProvider.MaximumTitleLength.Returns(EmbedBuilder.MaxTitleLength);
        limitProvider.MaximumRecipeLength.Returns(EmbedBuilder.MaxEmbedLength);
        limitProvider.MaximumFieldNameLength.Returns(EmbedFieldBuilder.MaxFieldNameLength);
        limitProvider.MaximumFieldDataLength.Returns(EmbedFieldBuilder.MaxFieldValueLength);
        limitProvider.MaximumRecipeTagsLength.Returns(EmbedFooterBuilder.MaxFooterTextLength);
        return limitProvider;
    }
}