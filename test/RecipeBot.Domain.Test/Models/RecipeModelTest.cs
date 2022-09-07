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
using RecipeBot.Domain.Models;
using RecipeBot.TestUtils;
using Xunit;

namespace RecipeBot.Domain.Test.Models;

public class RecipeModelTest
{
    private const string imageUrl = "http://just.an.image";

    [Theory]
    [ClassData(typeof(NullOrWhitespacesStringValueGenerator))]
    public void Model_with_invalid_recipe_title_throws_exception(string invalidRecipeTitle)
    {
        // Setup
        AuthorModel authorModel = CreateAuthor();

        // Call
        Action call = () => new RecipeModel(authorModel, Enumerable.Empty<RecipeFieldModel>(), invalidRecipeTitle);

        // Assert
        Assert.Throws<ArgumentException>(call);
    }

    [Theory]
    [ClassData(typeof(NullOrWhitespacesStringValueGenerator))]
    public void Model_with_image_url_and_invalid_title_throws_exception(string invalidRecipeTitle)
    {
        // Setup
        AuthorModel authorModel = CreateAuthor();

        // Call
        Action call = () => new RecipeModel(authorModel, Enumerable.Empty<RecipeFieldModel>(), invalidRecipeTitle, imageUrl);

        // Assert
        Assert.Throws<ArgumentException>(call);
    }

    [Fact]
    public void Model_without_image_url_sets_image_url_null()
    {
        // Setup
        AuthorModel authorModel = CreateAuthor();

        // Call
        var recipe = new RecipeModel(authorModel, Enumerable.Empty<RecipeFieldModel>(), "recipeTitle");

        // Assert
        Assert.Null(recipe.RecipeImageUrl);
    }

    [Fact]
    public void Model_with_valid_image_url_sets_image_url()
    {
        // Setup
        AuthorModel authorModel = CreateAuthor();

        // Call
        var recipe = new RecipeModel(authorModel, Enumerable.Empty<RecipeFieldModel>(), "recipeTitle", imageUrl);

        // Assert
        Assert.Equal(imageUrl, recipe.RecipeImageUrl);
    }

    [Theory]
    [ClassData(typeof(InvalidHttpUrlDataGenerator))]
    public void Model_with_invalid_image_url_throws_exception(string invalidImageUrl)
    {
        // Setup
        AuthorModel authorModel = CreateAuthor();

        // Call
        Action call = () => new RecipeModel(authorModel, Enumerable.Empty<RecipeFieldModel>(), "recipeTitle", invalidImageUrl);

        // Assert
        Assert.Throws<ArgumentException>(call);
    }

    [Theory]
    [InlineData("recipeTitle")]
    [InlineData("recipe title")]
    [InlineData("recipe    title")]
    [InlineData("     recipeTitle")]
    [InlineData("recipeTitle     ")]
    public void Model_with_valid_title_returns_total_length_of_properties(string recipeTitle)
    {
        // Setup
        var random = new Random(21);

        AuthorModel authorModel = CreateAuthor();
        IEnumerable<RecipeFieldModel> recipeFieldModels = new[]
        {
            CreateFields(random.Next()),
            CreateFields(random.Next()),
            CreateFields(random.Next())
        };

        var recipe = new RecipeModel(authorModel, recipeFieldModels, recipeTitle);

        // Call
        int totalLength = recipe.TotalLength;

        // Assert
        int expectedLength = recipeTitle.Length + authorModel.TotalLength + recipeFieldModels.Sum(f => f.TotalLength);
        Assert.Equal(expectedLength, totalLength);
    }

    [Theory]
    [InlineData("recipeTitle")]
    [InlineData("recipe title")]
    [InlineData("recipe    title")]
    [InlineData("     recipeTitle")]
    [InlineData("recipeTitle     ")]
    public void Model_with_valid_title_and_image_url_returns_total_length_of_properties(string recipeTitle)
    {
        // Setup
        var random = new Random(21);

        AuthorModel authorModel = CreateAuthor();
        IEnumerable<RecipeFieldModel> recipeFieldModels = new[]
        {
            CreateFields(random.Next()),
            CreateFields(random.Next()),
            CreateFields(random.Next())
        };

        var recipe = new RecipeModel(authorModel, recipeFieldModels, recipeTitle, imageUrl);

        // Call
        int totalLength = recipe.TotalLength;

        // Assert
        int expectedLength = recipeTitle.Length + authorModel.TotalLength + recipeFieldModels.Sum(f => f.TotalLength);
        Assert.Equal(expectedLength, totalLength);
    }

    [Fact]
    public void Model_with_empty_recipe_fields_returns_total_length_of_properties()
    {
        // Setup
        const string recipeTitle = "recipeTitle";

        AuthorModel authorModel = CreateAuthor();
        var recipe = new RecipeModel(authorModel, Enumerable.Empty<RecipeFieldModel>(), recipeTitle);

        // Call
        int totalLength = recipe.TotalLength;

        // Assert
        int expectedLength = recipeTitle.Length + authorModel.TotalLength;
        Assert.Equal(expectedLength, totalLength);
    }

    [Fact]
    public void Model_with_image_url_and_empty_recipe_fields_returns_total_length_of_properties()
    {
        // Setup
        const string recipeTitle = "recipeTitle";

        AuthorModel authorModel = CreateAuthor();
        var recipe = new RecipeModel(authorModel, Enumerable.Empty<RecipeFieldModel>(), recipeTitle, imageUrl);

        // Call
        int totalLength = recipe.TotalLength;

        // Assert
        int expectedLength = recipeTitle.Length + authorModel.TotalLength;
        Assert.Equal(expectedLength, totalLength);
    }

    private static AuthorModel CreateAuthor()
    {
        const string authorImageUrl = "http://www.google.com";

        var random = new Random(21);
        var authorName = new string('o', random.Next(100));

        return new AuthorModel(authorName, authorImageUrl);
    }

    private static RecipeFieldModel CreateFields(int seed)
    {
        var random = new Random(seed);
        var fieldName = new string('+', random.Next(100));
        var fieldData = new string('x', random.Next(100));

        return new RecipeFieldModel(fieldName, fieldData);
    }
}