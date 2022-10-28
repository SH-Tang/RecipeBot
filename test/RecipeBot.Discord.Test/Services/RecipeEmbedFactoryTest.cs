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

using System.Collections.Generic;
using System.Linq;
using Discord;
using RecipeBot.Domain.Data;
using RecipeBot.Discord.Services;
using RecipeBot.Domain.Models;
using RecipeBot.Domain.TestUtils;
using Xunit;

namespace RecipeBot.Discord.Test.Services;

public class RecipeEmbedFactoryTest
{
    private readonly RecipeDomainModelTestFactory domainTestFactory;

    public RecipeEmbedFactoryTest()
    {
        domainTestFactory = new RecipeDomainModelTestFactory(new RecipeDomainModelTestFactory.ConstructionProperties
        {
            MaxAuthorNameLength = EmbedAuthorBuilder.MaxAuthorNameLength,
            MaxTitleLength = EmbedBuilder.MaxTitleLength,
            MaxFieldNameLength = EmbedFieldBuilder.MaxFieldNameLength,
            MaxFieldDataLength = EmbedFieldBuilder.MaxFieldValueLength
        });
    }

    [Theory]
    [MemberData(nameof(GetRecipeCategoriesAndColor))]
    public void Basic_recipe_with_category_returns_embed_with_color(
        RecipeCategory category, Color expectedColor)
    {
        // Setup
        RecipeModel recipeModel = domainTestFactory.Create(category);

        // Call
        Embed embed = RecipeEmbedFactory.Create(recipeModel);

        // Assert
        Assert.Equal(expectedColor, embed.Color);
    }

    [Fact]
    public void Basic_recipe_should_return_embed_without_image_and_fields()
    {
        // Setup
        RecipeModel recipeModel = domainTestFactory.Create();

        // Call
        Embed embed = RecipeEmbedFactory.Create(recipeModel);

        // Assert
        Assert.Equal(recipeModel.Title, embed.Title);
        Assert.Null(embed.Image);

        AssertAuthor(recipeModel.Author, embed.Author);
        AssertFields(recipeModel.RecipeFields, embed.Fields);
    }

    [Fact]
    public void Recipe_with_fields_and_image_should_return_embed_with_fields_and_image()
    {
        // Setup
        RecipeModel recipeModel = domainTestFactory.CreateWithImageAndFields();

        // Call
        Embed embed = RecipeEmbedFactory.Create(recipeModel);

        // Assert
        Assert.Equal(recipeModel.Title, embed.Title);

        EmbedImage? embedImage = embed.Image;
        Assert.NotNull(embedImage);
        Assert.Equal(recipeModel.RecipeImageUrl, embedImage!.Value.Url);

        AssertAuthor(recipeModel.Author, embed.Author);
        AssertFields(recipeModel.RecipeFields, embed.Fields);
    }

    [Fact]
    public void Recipe_with_image_should_return_embed_with_image()
    {
        // Setup
        RecipeModel recipeModel = domainTestFactory.CreateWithImage();

        // Call
        Embed embed = RecipeEmbedFactory.Create(recipeModel);

        // Assert
        Assert.Equal(recipeModel.Title, embed.Title);

        EmbedImage? embedImage = embed.Image;
        Assert.NotNull(embedImage);
        Assert.Equal(recipeModel.RecipeImageUrl, embedImage!.Value.Url);

        AssertAuthor(recipeModel.Author, embed.Author);
        AssertFields(recipeModel.RecipeFields, embed.Fields);
    }

    [Fact]
    public void Recipe_with_fields_should_return_embed_with_fields()
    {
        // Setup
        RecipeModel recipeModel = domainTestFactory.CreateWithFields();

        // Call
        Embed embed = RecipeEmbedFactory.Create(recipeModel);

        // Assert
        Assert.Equal(recipeModel.Title, embed.Title);

        EmbedImage? embedImage = embed.Image;
        Assert.Null(embedImage);

        AssertAuthor(recipeModel.Author, embed.Author);
        AssertFields(recipeModel.RecipeFields, embed.Fields);
    }

    private static void AssertAuthor(AuthorModel authorData, EmbedAuthor? actualAuthor)
    {
        Assert.NotNull(actualAuthor);
        Assert.Equal(authorData.AuthorName, actualAuthor!.Value.Name);
        Assert.Equal(authorData.AuthorImageUrl, actualAuthor.Value.IconUrl);
    }

    private static void AssertFields(IEnumerable<RecipeFieldModel> recipeFields, IEnumerable<EmbedField> embedFields)
    {
        int nrOfRecipeFields = recipeFields.Count();
        Assert.Equal(nrOfRecipeFields, embedFields.Count());
        for (var i = 0; i < nrOfRecipeFields; i++)
        {
            AssertField(recipeFields.ElementAt(i), embedFields.ElementAt(i));
        }
    }

    private static void AssertField(RecipeFieldModel model, EmbedField actualField)
    {
        Assert.Equal(model.FieldName, actualField.Name);
        Assert.Equal(model.FieldData, actualField.Value);
        Assert.False(actualField.Inline);
    }

    private static IEnumerable<object[]> GetRecipeCategoriesAndColor()
    {
        yield return new object[]
        {
            RecipeCategory.Meat,
            new Color(250, 85, 87)
        };

        yield return new object[]
        {
            RecipeCategory.Fish,
            new Color(141, 223, 220)
        };

        yield return new object[]
        {
            RecipeCategory.Vegetarian,
            new Color(206, 221, 85)
        };
        yield return new object[]
        {
            RecipeCategory.Vegan,
            new Color(104, 115, 57)
        };
        yield return new object[]
        {
            RecipeCategory.Drinks,
            new Color(175, 234, 224)
        };
        yield return new object[]
        {
            RecipeCategory.Pastry,
            new Color(250, 207, 113)
        };
        yield return new object[]
        {
            RecipeCategory.Dessert,
            new Color(252, 238, 190)
        };
        yield return new object[]
        {
            RecipeCategory.Snack,
            new Color(249, 162, 114)
        };
        yield return new object[]
        {
            RecipeCategory.Other,
            new Color(204, 204, 203)
        };
    }
}