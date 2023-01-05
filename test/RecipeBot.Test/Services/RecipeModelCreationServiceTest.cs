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
using System.Linq;
using AutoFixture;
using Discord;
using NSubstitute;
using RecipeBot.Discord.Data;
using RecipeBot.Discord.TestUtils;
using RecipeBot.Discord.Views;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Factories;
using RecipeBot.Domain.Models;
using RecipeBot.Domain.TestUtils;
using RecipeBot.Exceptions;
using RecipeBot.Services;
using Xunit;

namespace RecipeBot.Test.Services;

public class RecipeModelCreationServiceTest
{
    [Fact]
    public void Recipe_with_invalid_data_and_valid_category_throws_exception()
    {
        // Setup
        var fixture = new Fixture();
        var category = fixture.Create<DiscordRecipeCategory>();

        var user = Substitute.For<IUser>();
        user.Username.Returns("Recipe author");
        user.GetAvatarUrl().ReturnsForAnyArgs("https://AuthorImage.url");

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
        limitProvider.MaximumAuthorNameLength.Returns(EmbedAuthorBuilder.MaxAuthorNameLength);
        limitProvider.MaximumFieldNameLength.Returns(EmbedFieldBuilder.MaxFieldNameLength);
        limitProvider.MaximumFieldDataLength.Returns(EmbedFieldBuilder.MaxFieldValueLength);
        limitProvider.MaximumRecipeTagsLength.Returns(EmbedFooterBuilder.MaxFooterTextLength);

        var service = new RecipeModelCreationService(limitProvider);

        // Call
        Action call = () => service.CreateRecipeModel(modal, user, category);

        // Assert
        var exception = Assert.Throws<ModelCreateException>(call);
    }

    [Theory]
    [MemberData(nameof(GetRecipeCategories))]
    public void Recipe_with_valid_data_returns_expected_response(DiscordRecipeCategory category)
    {
        // Setup
        const string authorName = "Recipe author";
        const string authorImageUrl = "https://AuthorImage.url";
        var user = Substitute.For<IUser>();
        user.Username.Returns(authorName);
        user.GetAvatarUrl().ReturnsForAnyArgs(authorImageUrl);

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
        AssertCommonModelProperties(user, category, modal, model);
        Assert.Null(model.RecipeImageUrl);
    }

    [Fact]
    public void Recipe_with_valid_attachment_and_category_and_invalid_data_throws_exception()
    {
        // Setup
        var fixture = new Fixture();
        var category = fixture.Create<DiscordRecipeCategory>();

        var user = Substitute.For<IUser>();
        user.Username.Returns("Recipe author");
        user.GetAvatarUrl().ReturnsForAnyArgs("https://AuthorImage.url");

        var attachment = Substitute.For<IAttachment>();
        attachment.ContentType.Returns("image/");
        attachment.Url.Returns("https://RecipeImage.url");

        var modal = new RecipeModal
        {
            RecipeTitle = "Recipe title",
            Ingredients = "My ingredients",
            CookingSteps = "My recipe steps",
            Notes = "My notes",
            Tags = "tag1, tag2"
        };

        var limitProvider = Substitute.For<IRecipeModelCharacterLimitProvider>();
        limitProvider.MaximumTitleLength.Returns(EmbedBuilder.MaxTitleLength);
        limitProvider.MaximumRecipeLength.Returns(0);
        limitProvider.MaximumAuthorNameLength.Returns(EmbedAuthorBuilder.MaxAuthorNameLength);
        limitProvider.MaximumFieldNameLength.Returns(EmbedFieldBuilder.MaxFieldNameLength);
        limitProvider.MaximumFieldDataLength.Returns(EmbedFieldBuilder.MaxFieldValueLength);
        limitProvider.MaximumRecipeTagsLength.Returns(EmbedFooterBuilder.MaxFooterTextLength);

        var service = new RecipeModelCreationService(limitProvider);

        // Call
        Action call = () => service.CreateRecipeModel(modal, user, category, attachment);

        // Assert
        var exception = Assert.Throws<ModalResponseException>(call);
        Exception? innerException = exception.InnerException;
        Assert.NotNull(innerException);
        Assert.Equal(innerException!.Message, exception.Message);
    }

    [Fact]
    public void Recipe_with_valid_attachment_and_valid_data_and_invalid_category_throws_exception()
    {
        // Setup
        const DiscordRecipeCategory category = (DiscordRecipeCategory)(-1);

        var user = Substitute.For<IUser>();
        user.Username.Returns("Recipe author");
        user.GetAvatarUrl().ReturnsForAnyArgs("https://AuthorImage.url");

        var attachment = Substitute.For<IAttachment>();
        attachment.ContentType.Returns("image/");
        attachment.Url.Returns("https://RecipeImage.url");

        var modal = new RecipeModal
        {
            RecipeTitle = "Recipe title",
            Ingredients = "My ingredients",
            CookingSteps = "My recipe steps",
            Notes = "My notes",
            Tags = "Tag1, Tag2"
        };

        IRecipeModelCharacterLimitProvider limitProvider = CreateDiscordCharacterLimitProvider();

        var service = new RecipeModelCreationService(limitProvider);

        // Call
        Action call = () => service.CreateRecipeModel(modal, user, category, attachment);

        // Assert
        var exception = Assert.Throws<ModalResponseException>(call);
        Exception? innerException = exception.InnerException;
        Assert.NotNull(innerException);
        Assert.Equal(innerException!.Message, exception.Message);
    }

    [Theory]
    [MemberData(nameof(GetRecipeCategories))]
    public void Recipe_with_valid_data_and_attachment_returns_expected_response(DiscordRecipeCategory category)
    {
        // Setup
        const string authorName = "Recipe author";
        const string authorImageUrl = "https://AuthorImage.url";
        var user = Substitute.For<IUser>();
        user.Username.Returns(authorName);
        user.GetAvatarUrl().ReturnsForAnyArgs(authorImageUrl);

        const string recipeImageUrl = "https://RecipeImage.url";
        var attachment = Substitute.For<IAttachment>();
        attachment.ContentType.Returns("image/");
        attachment.Url.Returns(recipeImageUrl);

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
        RecipeModel recipeModel = service.CreateRecipeModel(modal, user, category, attachment);

        // Assert
        AssertCommonModelProperties(user, category, modal, recipeModel);
        Assert.Equal(recipeImageUrl, recipeModel.RecipeImageUrl);
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
        limitProvider.MaximumAuthorNameLength.Returns(EmbedAuthorBuilder.MaxAuthorNameLength);
        limitProvider.MaximumFieldNameLength.Returns(EmbedFieldBuilder.MaxFieldNameLength);
        limitProvider.MaximumFieldDataLength.Returns(EmbedFieldBuilder.MaxFieldValueLength);
        limitProvider.MaximumRecipeTagsLength.Returns(EmbedFooterBuilder.MaxFooterTextLength);
        return limitProvider;
    }

    private static void AssertCommonModelProperties(IUser user, DiscordRecipeCategory category,
                                                    RecipeModal modal, RecipeModel actualRecipe)
    {
        Assert.Equal(DiscordRecipeCategoryHelper.RecipeCategoryMapping[category], actualRecipe.RecipeCategory);

        AuthorModel actualAuthor = actualRecipe.Author;
        Assert.NotNull(actualAuthor);
        AssertAuthor(user.Username, user.GetAvatarUrl(), actualAuthor);

        Assert.Equal(modal.RecipeTitle, actualRecipe.Title);

        Assert.Equal(3, actualRecipe.RecipeFields.Count());
        AssertField("Ingredients", modal.Ingredients, actualRecipe.RecipeFields.ElementAt(0));
        AssertField("Cooking steps", modal.CookingSteps, actualRecipe.RecipeFields.ElementAt(1));
        AssertField("Additional notes", modal.Notes, actualRecipe.RecipeFields.ElementAt(2));

        AssertTags(modal.Tags, actualRecipe.RecipeTags);
    }

    private static void AssertAuthor(string expectedAuthorName, string expectedAuthorImageUrl, AuthorModel actualAuthor)
    {
        Assert.Equal(expectedAuthorName, actualAuthor.AuthorName);
        Assert.Equal(expectedAuthorImageUrl, actualAuthor.AuthorImageUrl);
    }

    private static void AssertField(string expectedName, string? expectedValue, RecipeFieldModel actualField)
    {
        Assert.Equal(expectedName, actualField.FieldName);
        Assert.Equal(expectedValue, actualField.FieldData);
    }

    private static void AssertTags(string? tags, RecipeTagsModelWrapper actualTags)
    {
        IEnumerable<string> expectedTags = TagTestHelper.GetParsedTags(tags);
        Assert.Equal(expectedTags, actualTags.Tags);
    }
}