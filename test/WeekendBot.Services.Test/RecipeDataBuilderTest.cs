// Copyright (C) 2022 Dennis Tang. All rights reserved.
//
// This file is part of WeekendBot.
//
// WeekendBot is free software: you can redistribute it and/or modify
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
using Discord;
using Discord.Common.Utils;
using NSubstitute;
using WeekendBot.TestUtils;
using Xunit;

namespace WeekendBot.Services.Test;

public class RecipeDataBuilderTest
{
    [Fact]
    public void Builder_without_adding_notes_and_attachment_then_build_recipe_data_without_notes_and_image_url()
    {
        // Setup
        AuthorData authorData = CreateValidAuthorData();

        const string recipeTitle = "Recipe title";
        const string recipeIngredients = "My ingredients";
        const string cookingSteps = "My recipe steps";

        var builder = new RecipeDataBuilder(authorData, recipeTitle, recipeIngredients, cookingSteps);

        // Call
        RecipeData result = builder.Build();

        // Assert
        AssertRecipeCommonProperties(recipeTitle, recipeIngredients, cookingSteps, authorData, result);
        Assert.Null(result.AdditionalNotes);
        Assert.Null(result.ImageUrl);
    }


    [Theory]
    [InlineData(null)]
    [InlineData("    ")]
    [InlineData("")]
    [InlineData("Notes")]
    public void Builder_when_adding_notes_then_build_recipe_data_with_notes(string notes)
    {
        // Setup
        AuthorData authorData = CreateValidAuthorData();

        const string recipeTitle = "Recipe title";
        const string recipeIngredients = "My ingredients";
        const string cookingSteps = "My recipe steps";

        var builder = new RecipeDataBuilder(authorData, recipeTitle, recipeIngredients, cookingSteps);

        // Call
        RecipeData result = builder.AddNotes(notes)
                                   .Build();

        // Assert
        AssertRecipeCommonProperties(recipeTitle, recipeIngredients, cookingSteps, authorData, result);
        Assert.Equal(notes, result.AdditionalNotes);
    }

    [Theory]
    [MemberData(nameof(GetValidImageAttachments))]
    public void Builder_when_adding_valid_image_attachment_then_build_recipe_data_with_image_url
        (IAttachment attachment, string expectedRecipeImageUrl)
    {
        // Setup
        AuthorData authorData = CreateValidAuthorData();

        const string recipeTitle = "Recipe title";
        const string recipeIngredients = "My ingredients";
        const string cookingSteps = "My recipe steps";
        var builder = new RecipeDataBuilder(authorData, recipeTitle, recipeIngredients, cookingSteps);

        // Call
        RecipeData result = builder.AddImage(attachment)
                                   .Build();

        // Assert
        AssertRecipeCommonProperties(recipeTitle, recipeIngredients, cookingSteps, authorData, result);
        Assert.Equal(expectedRecipeImageUrl, result.ImageUrl);
    }

    [Theory]
    [ClassData(typeof(EmptyOrNullStringValueGenerator))]
    [InlineData("notImage/")]
    public void Builder_when_adding_invalid_image_attachment_then_throws_exception(string invalidContentType)
    {
        // Setup
        var attachment = Substitute.For<IAttachment>();
        attachment.ContentType.Returns(invalidContentType);

        AuthorData authorData = CreateValidAuthorData();

        const string recipeTitle = "Recipe title";
        const string recipeIngredients = "My ingredients";
        const string cookingSteps = "My recipe steps";
        var builder = new RecipeDataBuilder(authorData, recipeTitle, recipeIngredients, cookingSteps);

        // Precondition
        Assert.False(attachment.IsImage());

        // Call
        Func<RecipeDataBuilder> call = () => builder.AddImage(attachment);

        // Assert
        Assert.Throws<ArgumentException>(call);
    }

    private static IEnumerable<object?[]> GetValidImageAttachments()
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

    private static AuthorData CreateValidAuthorData()
    {
        const string authorName = "Recipe author";
        const string authorImageUrl = "https://AuthorImage.url";
        var authorData = new AuthorData(authorName, authorImageUrl);
        return authorData;
    }

    private static void AssertRecipeCommonProperties(
        string expectedRecipeTitle, string expectedRecipeIngredients, string expectedCookingSteps, AuthorData expectedAuthorData,
        RecipeData actualRecipeData)
    {
        Assert.Equal(expectedRecipeTitle, actualRecipeData.RecipeTitle);
        Assert.Equal(expectedRecipeIngredients, actualRecipeData.RecipeIngredients);
        Assert.Equal(expectedCookingSteps, actualRecipeData.CookingSteps);

        Assert.Same(expectedAuthorData, actualRecipeData.AuthorData);
    }
}