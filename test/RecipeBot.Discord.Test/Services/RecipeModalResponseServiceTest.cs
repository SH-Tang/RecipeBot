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
using RecipeBot.Discord.Exceptions;
using RecipeBot.Discord.Services;
using RecipeBot.Discord.Views;
using RecipeBot.Domain.Factories;
using Xunit;

namespace RecipeBot.Discord.Test.Services;

public class RecipeModalResponseServiceTest
{
    private static readonly Dictionary<DiscordRecipeCategory, string> categoryMapping = new()
    {
        {
            DiscordRecipeCategory.Meat, "Meat"
        },
        {
            DiscordRecipeCategory.Fish, "Fish"
        },
        {
            DiscordRecipeCategory.Vegetarian, "Vegetarian"
        },
        {
            DiscordRecipeCategory.Vegan, "Vegan"
        },
        {
            DiscordRecipeCategory.Drinks, "Drinks"
        },
        {
            DiscordRecipeCategory.Pastry, "Pastry"
        },
        {
            DiscordRecipeCategory.Dessert, "Dessert"
        },
        {
            DiscordRecipeCategory.Snack, "Snack"
        },
        {
            DiscordRecipeCategory.Other, "Other"
        }
    };

    [Fact]
    public void Recipe_with_valid_data_and_invalid_category_throws_exception()
    {
        // Setup
        const DiscordRecipeCategory category = (DiscordRecipeCategory)(-1);

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
            Notes = "My notes",
            Tags = "Tag1, Tag2, Tag1"
        };

        var limitProvider = Substitute.For<IRecipeModelCharacterLimitProvider>();
        limitProvider.MaximumTitleLength.Returns(EmbedBuilder.MaxTitleLength);
        limitProvider.MaximumAuthorNameLength.Returns(EmbedAuthorBuilder.MaxAuthorNameLength);
        limitProvider.MaximumFieldNameLength.Returns(EmbedFieldBuilder.MaxFieldNameLength);
        limitProvider.MaximumFieldDataLength.Returns(EmbedFieldBuilder.MaxFieldValueLength);
        limitProvider.MaximumRecipeTagsLength.Returns(EmbedFooterBuilder.MaxFooterTextLength);

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
        const string tags = "Tag1, Tag2, Tag1";
        var modal = new RecipeModal
        {
            RecipeTitle = recipeTitle,
            Ingredients = recipeIngredients,
            CookingSteps = recipeSteps,
            Notes = recipeNotes,
            Tags = tags
        };

        IRecipeModelCharacterLimitProvider limitProvider = CreateDiscordCharacterLimitProvider();
        var service = new RecipeModalResponseService(limitProvider);

        // Call
        Embed response = service.GetRecipeModalResponse(modal, user, category);

        // Assert
        AssertCommonEmbedResponseProperties(user, category, modal, expectedColor, response);
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
        const string tags = "Tag1, Tag2, Tag1";
        var modal = new RecipeModal
        {
            RecipeTitle = recipeTitle,
            Ingredients = recipeIngredients,
            CookingSteps = recipeSteps,
            Notes = recipeNotes,
            Tags = tags
        };

        IRecipeModelCharacterLimitProvider limitProvider = CreateDiscordCharacterLimitProvider();
        var service = new RecipeModalResponseService(limitProvider);

        // Call
        Embed response = service.GetRecipeModalResponse(modal, user, category, attachment);

        // Assert
        AssertCommonEmbedResponseProperties(user, category, modal, expectedColor, response);

        EmbedImage? embedImage = response.Image;
        Assert.NotNull(embedImage);
        EmbedImage resultImage = embedImage!.Value;
        Assert.Equal(recipeImageUrl, resultImage.Url);
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

    private static void AssertCommonEmbedResponseProperties(
        IUser user, DiscordRecipeCategory category, RecipeModal modal, Color color, IEmbed actualResponse)
    {
        EmbedAuthor? actualResponseAuthor = actualResponse.Author;
        Assert.NotNull(actualResponseAuthor);
        AssertAuthor(user.Username, user.GetAvatarUrl(), actualResponseAuthor.Value);

        Assert.Equal(modal.RecipeTitle, actualResponse.Title);

        Assert.Equal(3, actualResponse.Fields.Length);
        AssertField("Ingredients", modal.Ingredients, actualResponse.Fields[0]);
        AssertField("Cooking steps", modal.CookingSteps, actualResponse.Fields[1]);
        AssertField("Additional notes", modal.Notes, actualResponse.Fields[2]);

        Assert.Equal(color, actualResponse.Color);

        EmbedFooter? actualResponseFooter = actualResponse.Footer;
        Assert.NotNull(actualResponseFooter);
        AssertTags(category, modal.Tags, actualResponseFooter.Value);
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

    private static void AssertTags(DiscordRecipeCategory category, string tags, EmbedFooter actualFooter)
    {
        var expectedTags = new List<string>
        {
            categoryMapping[category]
        };

        if (!string.IsNullOrWhiteSpace(tags))
        {
            IEnumerable<string> parsedTags = tags.Split(",").Select(t => t.Trim()).Distinct();
            expectedTags.AddRange(parsedTags);
        }

        string expectedFooterText = string.Join(", ", expectedTags);
        Assert.Equal(expectedFooterText, actualFooter.Text);
    }
}