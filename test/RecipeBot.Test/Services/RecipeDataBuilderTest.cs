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
using AutoFixture;
using Discord;
using Discord.Common.Utils;
using NSubstitute;
using RecipeBot.Discord.Data;
using RecipeBot.Domain.Data;
using RecipeBot.Services;
using RecipeBot.TestUtils;
using Xunit;

namespace RecipeBot.Test.Services;

public class RecipeDataBuilderTest
{
    [Theory]
    [InlineData(DiscordRecipeCategory.Dessert, RecipeCategory.Dessert)]
    [InlineData(DiscordRecipeCategory.Fish, RecipeCategory.Fish)]
    [InlineData(DiscordRecipeCategory.Meat, RecipeCategory.Meat)]
    [InlineData(DiscordRecipeCategory.Pastry, RecipeCategory.Pastry)]
    [InlineData(DiscordRecipeCategory.Snack, RecipeCategory.Snack)]
    [InlineData(DiscordRecipeCategory.Vegan, RecipeCategory.Vegan)]
    [InlineData(DiscordRecipeCategory.Vegetarian, RecipeCategory.Vegetarian)]
    [InlineData(DiscordRecipeCategory.Drinks, RecipeCategory.Drinks)]
    [InlineData(DiscordRecipeCategory.Other, RecipeCategory.Other)]
    public void Builder_with_recipe_category_builds_recipe_data_with_expected_category(
        DiscordRecipeCategory discordCategory, RecipeCategory expectedCategory)
    {
        // Setup
        var fixture = new Fixture();

        var authorData = fixture.Create<AuthorData>();
        var recipeTitle = fixture.Create<string>();
        var recipeIngredients = fixture.Create<string>();
        var cookingSteps = fixture.Create<string>();

        var builder = new RecipeDataBuilder(authorData, discordCategory, recipeTitle, recipeIngredients, cookingSteps);

        // Call
        RecipeData result = builder.Build();

        // Assert
        Assert.Equal(expectedCategory, result.Category);
    }

    [Fact]
    public void Builder_with_invalid_recipe_category_throws_exception()
    {
        // Setup
        var fixture = new Fixture();

        var authorData = fixture.Create<AuthorData>();
        var recipeTitle = fixture.Create<string>();
        var recipeIngredients = fixture.Create<string>();
        var cookingSteps = fixture.Create<string>();

        const DiscordRecipeCategory discordCategory = (DiscordRecipeCategory)(-1);

        // Call
        Action call = () => new RecipeDataBuilder(authorData, discordCategory, recipeTitle, recipeIngredients, cookingSteps);

        // Assert
        Assert.Throws<InvalidEnumArgumentException>(call);
    }

    [Fact]
    public void Builder_without_optional_data_then_build_recipe_data_with_mandatory_fields()
    {
        // Setup
        var fixture = new Fixture();

        var authorData = fixture.Create<AuthorData>();
        var discordCategory = fixture.Create<DiscordRecipeCategory>();
        var recipeTitle = fixture.Create<string>();
        var recipeIngredients = fixture.Create<string>();
        var cookingSteps = fixture.Create<string>();

        var builder = new RecipeDataBuilder(authorData, discordCategory, recipeTitle, recipeIngredients, cookingSteps);

        // Call
        RecipeData result = builder.Build();

        // Assert
        AssertMandatoryRecipeProperties(recipeTitle, recipeIngredients, cookingSteps, authorData, result);
        Assert.Null(result.AdditionalNotes);
        Assert.Null(result.ImageUrl);
        Assert.Null(result.Tags);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("    ")]
    [InlineData("")]
    [InlineData("Notes")]
    public void Builder_when_adding_notes_then_build_recipe_data_with_notes(string notes)
    {
        // Setup
        var fixture = new Fixture();

        var authorData = fixture.Create<AuthorData>();
        var discordCategory = fixture.Create<DiscordRecipeCategory>();
        var recipeTitle = fixture.Create<string>();
        var recipeIngredients = fixture.Create<string>();
        var cookingSteps = fixture.Create<string>();

        var builder = new RecipeDataBuilder(authorData, discordCategory, recipeTitle, recipeIngredients, cookingSteps);

        // Call
        RecipeData result = builder.AddNotes(notes)
                                   .Build();

        // Assert
        AssertMandatoryRecipeProperties(recipeTitle, recipeIngredients, cookingSteps, authorData, result);
        Assert.Equal(notes, (string?)result.AdditionalNotes);
    }

    [Theory]
    [MemberData(nameof(GetValidImageAttachments))]
    public void Builder_when_adding_valid_image_attachment_then_build_recipe_data_with_image_url
        (IAttachment attachment, string expectedRecipeImageUrl)
    {
        // Setup
        var fixture = new Fixture();

        var authorData = fixture.Create<AuthorData>();
        var discordCategory = fixture.Create<DiscordRecipeCategory>();
        var recipeTitle = fixture.Create<string>();
        var recipeIngredients = fixture.Create<string>();
        var cookingSteps = fixture.Create<string>();

        var builder = new RecipeDataBuilder(authorData, discordCategory, recipeTitle, recipeIngredients, cookingSteps);

        // Call
        RecipeData result = builder.AddImage(attachment)
                                   .Build();

        // Assert
        AssertMandatoryRecipeProperties(recipeTitle, recipeIngredients, cookingSteps, authorData, result);
        Assert.Equal(expectedRecipeImageUrl, (string?)result.ImageUrl);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("    ")]
    [InlineData("")]
    [InlineData("tags")]
    public void Builder_when_adding_tags_then_build_recipe_data_with_tags(string tags)
    {
        // Setup
        var fixture = new Fixture();

        var authorData = fixture.Create<AuthorData>();
        var discordCategory = fixture.Create<DiscordRecipeCategory>();
        var recipeTitle = fixture.Create<string>();
        var recipeIngredients = fixture.Create<string>();
        var cookingSteps = fixture.Create<string>();

        var builder = new RecipeDataBuilder(authorData, discordCategory, recipeTitle, recipeIngredients, cookingSteps);

        // Call
        RecipeData result = builder.AddTags(tags)
                                   .Build();

        // Assert
        AssertMandatoryRecipeProperties(recipeTitle, recipeIngredients, cookingSteps, authorData, result);
        Assert.Equal(tags, (string?)result.Tags);
    }

    [Fact]
    public void Builder_with_optional_data_then_build_recipe_data_with_all_properties()
    {
        // Setup
        var fixture = new Fixture();

        var authorData = fixture.Create<AuthorData>();
        var discordCategory = fixture.Create<DiscordRecipeCategory>();
        var recipeTitle = fixture.Create<string>();
        var recipeIngredients = fixture.Create<string>();
        var cookingSteps = fixture.Create<string>();
        var notes = fixture.Create<string>();
        var tags = fixture.Create<string>();

        const string recipeImageUrl = "https://RecipeImage.url";
        var attachment = Substitute.For<IAttachment>();
        attachment.ContentType.Returns("image/");
        attachment.Url.Returns(recipeImageUrl);

        var builder = new RecipeDataBuilder(authorData, discordCategory, recipeTitle, recipeIngredients, cookingSteps);

        // Call
        RecipeData result = builder.AddNotes(notes)
                                   .AddTags(tags)
                                   .AddImage(attachment)
                                   .Build();

        // Assert
        AssertMandatoryRecipeProperties(recipeTitle, recipeIngredients, cookingSteps, authorData, result);
        Assert.Equal(notes, result.AdditionalNotes);
        Assert.Equal((string?)recipeImageUrl, (string?)result.ImageUrl);
        Assert.Equal(tags, result.Tags);
    }

    [Theory]
    [ClassData(typeof(NullOrWhitespacesStringValueGenerator))]
    [InlineData("notImage/")]
    public void Builder_when_adding_invalid_image_attachment_then_throws_exception(string invalidContentType)
    {
        // Setup
        var attachment = Substitute.For<IAttachment>();
        attachment.ContentType.Returns(invalidContentType);

        var fixture = new Fixture();

        var authorData = fixture.Create<AuthorData>();
        var discordCategory = fixture.Create<DiscordRecipeCategory>();
        var recipeTitle = fixture.Create<string>();
        var recipeIngredients = fixture.Create<string>();
        var cookingSteps = fixture.Create<string>();

        var builder = new RecipeDataBuilder(authorData, discordCategory, recipeTitle, recipeIngredients, cookingSteps);

        // Precondition
        Assert.False((bool)attachment.IsImage());

        // Call
        Action call = () => builder.AddImage(attachment);

        // Assert
        Assert.Throws<ArgumentException>(call);
    }

    public static IEnumerable<object?[]> GetValidImageAttachments()
    {
        yield return new object?[]
        {
            null,
            null
        };

        const string recipeImageUrl = "https://RecipeImage.url";
        var attachment = Substitute.For<IAttachment>();
        attachment.ContentType.Returns("image/");
        attachment.Url.Returns(recipeImageUrl);

        yield return new object?[]
        {
            attachment,
            recipeImageUrl
        };
    }

    private static void AssertMandatoryRecipeProperties(
        string expectedRecipeTitle, string expectedRecipeIngredients, string expectedCookingSteps, AuthorData expectedAuthorData,
        RecipeData actualRecipeData)
    {
        Assert.Equal(expectedRecipeTitle, (string?)actualRecipeData.RecipeTitle);
        Assert.Equal(expectedRecipeIngredients, (string?)actualRecipeData.RecipeIngredients);
        Assert.Equal(expectedCookingSteps, (string?)actualRecipeData.CookingSteps);

        Assert.Same(expectedAuthorData, actualRecipeData.AuthorData);
    }
}