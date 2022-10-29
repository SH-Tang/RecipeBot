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
using Discord;
using NSubstitute;
using RecipeBot.Discord.Data;
using RecipeBot.Discord.Exceptions;
using RecipeBot.Discord.Services;
using RecipeBot.Discord.Views;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Factories;
using Xunit;

namespace RecipeBot.Discord.Test.Services;

public class RecipeModalResponseServiceTest
{

    [Fact]
    public void Recipe_with_valid_data_and_invalid_category_throws_exception()
    {
        // Setup
        const DiscordRecipeCategory category = (DiscordRecipeCategory) (-1);

        var user = Substitute.For<IUser>();
        user.Username.Returns("Recipe author");
        user.GetAvatarUrl().ReturnsForAnyArgs("https://AuthorImage.url");

        var modal = new RecipeModal
        {
            RecipeTitle = "Recipe title",
            Ingredients = "My ingredients",
            CookingSteps = "My recipe steps",
            Notes = "My notes"
        };

        IRecipeModelCharacterLimitProvider limitProvider = CreateDiscordCharacterLimitProvider();

        var service = new RecipeModalResponseService(limitProvider);

        // Call
        Action call = () => service.GetRecipeModalResponse(modal, user, category);

        // Assert
        var exception = Assert.Throws<ModalResponseException>(call);
        Exception? innerException = exception.InnerException;
        Assert.NotNull(innerException);
        Assert.Equal(innerException!.Message, exception.Message);
    }

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
            Notes = "My notes"
        };

        var limitProvider = Substitute.For<IRecipeModelCharacterLimitProvider>();
        limitProvider.MaximumTitleLength.Returns(EmbedBuilder.MaxTitleLength);
        limitProvider.MaximumAuthorNameLength.Returns(EmbedAuthorBuilder.MaxAuthorNameLength);
        limitProvider.MaximumFieldNameLength.Returns(EmbedFieldBuilder.MaxFieldNameLength);
        limitProvider.MaximumFieldDataLength.Returns(EmbedFieldBuilder.MaxFieldValueLength);

        var service = new RecipeModalResponseService(limitProvider);

        // Call
        Action call = () => service.GetRecipeModalResponse(modal, user, category);

        // Assert
        var exception = Assert.Throws<ModalResponseException>(call);
        Exception? innerException = exception.InnerException;
        Assert.NotNull(innerException);
        Assert.Equal(innerException!.Message, exception.Message);
    }

    [Theory]
    [MemberData(nameof(GetRecipeCategoriesAndColor))]
    public void Recipe_with_valid_data_returns_expected_response(
        DiscordRecipeCategory category, Color expectedColor)
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
    
        var modal = new RecipeModal
        {
            RecipeTitle = recipeTitle,
            Ingredients = recipeIngredients,
            CookingSteps = recipeSteps,
            Notes = recipeNotes
        };
    
        IRecipeModelCharacterLimitProvider limitProvider = CreateDiscordCharacterLimitProvider();
        var service = new RecipeModalResponseService(limitProvider);
    
        // Call
        Embed response = service.GetRecipeModalResponse(modal, user, category);
    
        // Assert
        AssertCommonEmbedResponseProperties(user, modal, expectedColor, response);
        Assert.Null(response.Image);
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
            Notes = "My notes"
        };

        var limitProvider = Substitute.For<IRecipeModelCharacterLimitProvider>();
        limitProvider.MaximumTitleLength.Returns(EmbedBuilder.MaxTitleLength);
        limitProvider.MaximumRecipeLength.Returns(0);
        limitProvider.MaximumAuthorNameLength.Returns(EmbedAuthorBuilder.MaxAuthorNameLength);
        limitProvider.MaximumFieldNameLength.Returns(EmbedFieldBuilder.MaxFieldNameLength);
        limitProvider.MaximumFieldDataLength.Returns(EmbedFieldBuilder.MaxFieldValueLength);

        var service = new RecipeModalResponseService(limitProvider);

        // Call
        Action call = () => service.GetRecipeModalResponse(modal, user, category, attachment);

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
        const DiscordRecipeCategory category = (DiscordRecipeCategory) (-1);

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
            Notes = "My notes"
        };

        var limitProvider = CreateDiscordCharacterLimitProvider();

        var service = new RecipeModalResponseService(limitProvider);

        // Call
        Action call = () => service.GetRecipeModalResponse(modal, user, category, attachment);

        // Assert
        var exception = Assert.Throws<ModalResponseException>(call);
        Exception? innerException = exception.InnerException;
        Assert.NotNull(innerException);
        Assert.Equal(innerException!.Message, exception.Message);
    }

    [Theory]
    [MemberData(nameof(GetRecipeCategoriesAndColor))]
    public void Recipe_with_valid_data_and_attachment_returns_expected_response(
        DiscordRecipeCategory category, Color expectedColor)
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
        var modal = new RecipeModal
        {
            RecipeTitle = recipeTitle,
            Ingredients = recipeIngredients,
            CookingSteps = recipeSteps,
            Notes = recipeNotes
        };

        IRecipeModelCharacterLimitProvider limitProvider = CreateDiscordCharacterLimitProvider();
        var service = new RecipeModalResponseService(limitProvider);

        // Call
        Embed response = service.GetRecipeModalResponse(modal, user, category, attachment);

        // Assert
        AssertCommonEmbedResponseProperties(user, modal, expectedColor, response);

        EmbedImage? embedImage = response.Image;
        Assert.NotNull(embedImage);
        EmbedImage resultImage = embedImage!.Value;
        Assert.Equal(recipeImageUrl, resultImage.Url);
    }

    private static IRecipeModelCharacterLimitProvider CreateDiscordCharacterLimitProvider()
    {
        var limitProvider = Substitute.For<IRecipeModelCharacterLimitProvider>();
        limitProvider.MaximumTitleLength.Returns(EmbedBuilder.MaxTitleLength);
        limitProvider.MaximumRecipeLength.Returns(EmbedBuilder.MaxEmbedLength);
        limitProvider.MaximumAuthorNameLength.Returns(EmbedAuthorBuilder.MaxAuthorNameLength);
        limitProvider.MaximumFieldNameLength.Returns(EmbedFieldBuilder.MaxFieldNameLength);
        limitProvider.MaximumFieldDataLength.Returns(EmbedFieldBuilder.MaxFieldValueLength);
        return limitProvider;
    }

    private static void AssertCommonEmbedResponseProperties(IUser user, RecipeModal modal, Color color, IEmbed actualResponse)
    {
        EmbedAuthor? actualResponseAuthor = actualResponse.Author;
        Assert.NotNull(actualResponseAuthor);
        EmbedAuthor embedAuthor = actualResponseAuthor!.Value;
        AssertAuthor(user.Username, user.GetAvatarUrl(), embedAuthor);

        Assert.Equal(modal.RecipeTitle, actualResponse.Title);

        Assert.Equal(3, actualResponse.Fields.Length);
        AssertField("Ingredients", modal.Ingredients, actualResponse.Fields[0]);
        AssertField("Cooking steps", modal.CookingSteps, actualResponse.Fields[1]);
        AssertField("Additional notes", modal.Notes, actualResponse.Fields[2]);

        Assert.Equal(color, actualResponse.Color);
    }

    private static void AssertAuthor(string expectedAuthorName, string expectedAuthorImageUrl, EmbedAuthor actualAuthor)
    {
        Assert.Equal(expectedAuthorName, actualAuthor.Name);
        Assert.Equal(expectedAuthorImageUrl, actualAuthor.IconUrl);
    }

    private static void AssertField(string expectedName, string? expectedValue, EmbedField actualField)
    {
        Assert.Equal(expectedName, actualField.Name);
        Assert.Equal(expectedValue, actualField.Value);
        Assert.False(actualField.Inline);
    }

    public static IEnumerable<object[]> GetRecipeCategoriesAndColor()
    {
        yield return new object[]
        {
            DiscordRecipeCategory.Meat,
            new Color(250, 85, 87)
        };

        yield return new object[]
        {
            DiscordRecipeCategory.Fish,
            new Color(86, 153, 220)
        };

        yield return new object[]
        {
            DiscordRecipeCategory.Vegetarian,
            new Color(206, 221, 85)
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Vegan,
            new Color(6, 167, 125)
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Drinks,
            new Color(175, 234, 224)
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Pastry,
            new Color(206, 132, 173)
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Dessert,
            new Color(176, 69, 162)
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Snack,
            new Color(249, 162, 114)
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Other,
            new Color(165, 161, 164)
        };
    }
}