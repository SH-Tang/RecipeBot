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
using System.Linq;
using AutoFixture;
using RecipeBot.Domain.Data;
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
        var fixture = new Fixture();
        AuthorModel authorModel = CreateAuthor(fixture);
        var category = fixture.Create<RecipeCategory>();

        // Call
        Action call = () => new RecipeModel(authorModel, category, Enumerable.Empty<RecipeFieldModel>(), invalidRecipeTitle);

        // Assert
        Assert.Throws<ArgumentException>(call);
    }

    [Theory]
    [ClassData(typeof(NullOrWhitespacesStringValueGenerator))]
    public void Model_with_image_url_and_invalid_title_throws_exception(string invalidRecipeTitle)
    {
        // Setup
        var fixture = new Fixture();
        AuthorModel authorModel = CreateAuthor(fixture);
        var category = fixture.Create<RecipeCategory>();

        // Call
        Action call = () => new RecipeModel(authorModel, category, Enumerable.Empty<RecipeFieldModel>(), invalidRecipeTitle, imageUrl);

        // Assert
        Assert.Throws<ArgumentException>(call);
    }

    [Fact]
    public void Model_without_image_url_sets_image_url_null()
    {
        // Setup
        var fixture = new Fixture();
        var recipeTitle = fixture.Create<string>();
        AuthorModel authorModel = CreateAuthor(fixture);
        var category = fixture.Create<RecipeCategory>();

        // Call
        var recipe = new RecipeModel(authorModel, category, Enumerable.Empty<RecipeFieldModel>(), recipeTitle);

        // Assert
        Assert.Null(recipe.RecipeImageUrl);
    }

    [Fact]
    public void Model_with_valid_image_url_sets_image_url()
    {
        // Setup
        var fixture = new Fixture();
        var recipeTitle = fixture.Create<string>();
        AuthorModel authorModel = CreateAuthor(fixture);
        var category = fixture.Create<RecipeCategory>();

        // Call
        var recipe = new RecipeModel(authorModel, category, Enumerable.Empty<RecipeFieldModel>(), recipeTitle, imageUrl);

        // Assert
        Assert.Equal(imageUrl, recipe.RecipeImageUrl);
    }

    [Theory]
    [ClassData(typeof(InvalidHttpUrlDataGenerator))]
    public void Model_with_invalid_image_url_throws_exception(string invalidImageUrl)
    {
        // Setup
        var fixture = new Fixture();
        var recipeTitle = fixture.Create<string>();
        AuthorModel authorModel = CreateAuthor(fixture);
        var category = fixture.Create<RecipeCategory>();

        // Call
        Action call = () => new RecipeModel(authorModel, category, Enumerable.Empty<RecipeFieldModel>(), recipeTitle, invalidImageUrl);

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
        var fixture = new Fixture();
        AuthorModel authorModel = CreateAuthor(fixture);
        IEnumerable<RecipeFieldModel> recipeFields = new[]
        {
            CreateFields(fixture),
            CreateFields(fixture),
            CreateFields(fixture)
        };

        RecipeModel recipe = fixture.Build<RecipeModel>()
                                    .FromFactory<RecipeCategory>(category => new RecipeModel(authorModel, category, recipeFields, recipeTitle))
                                    .Create();

        // Call
        int totalLength = recipe.TotalLength;

        // Assert
        int expectedLength = recipeTitle.Length + authorModel.TotalLength + recipeFields.Sum(f => f.TotalLength);
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
        var fixture = new Fixture();
        AuthorModel authorModel = CreateAuthor(fixture);
        IEnumerable<RecipeFieldModel> recipeFields = new[]
        {
            CreateFields(fixture),
            CreateFields(fixture),
            CreateFields(fixture)
        };

        RecipeModel recipe = fixture.Build<RecipeModel>()
                                    .FromFactory<RecipeCategory>(category => new RecipeModel(authorModel, category, recipeFields, recipeTitle, imageUrl))
                                    .Create();

        // Call
        int totalLength = recipe.TotalLength;

        // Assert
        int expectedLength = recipeTitle.Length + authorModel.TotalLength + recipeFields.Sum(f => f.TotalLength);
        Assert.Equal(expectedLength, totalLength);
    }

    [Fact]
    public void Model_with_empty_recipe_fields_returns_total_length_of_properties()
    {
        // Setup
        var fixture = new Fixture();
        var recipeTitle = fixture.Create<string>();

        AuthorModel authorModel = CreateAuthor(fixture);
        RecipeModel recipe = fixture.Build<RecipeModel>()
                                    .FromFactory<RecipeCategory>(category => new RecipeModel(authorModel, category, Enumerable.Empty<RecipeFieldModel>(), recipeTitle))
                                    .Create();

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
        var fixture = new Fixture();
        var recipeTitle = fixture.Create<string>();

        AuthorModel authorModel = CreateAuthor(fixture);
        RecipeModel? recipe = fixture.Build<RecipeModel>()
                                     .FromFactory<RecipeCategory>(category => new RecipeModel(authorModel, category, Enumerable.Empty<RecipeFieldModel>(), recipeTitle, imageUrl))
                                     .Create();

        // Call
        int totalLength = recipe.TotalLength;

        // Assert
        int expectedLength = recipeTitle.Length + authorModel.TotalLength;
        Assert.Equal(expectedLength, totalLength);
    }

    private static AuthorModel CreateAuthor(Fixture fixture)
    {
        return fixture.Build<AuthorModel>()
                      .FromFactory<string>(author => new AuthorModel(author, imageUrl))
                      .Create(); ;
    }

    private static RecipeFieldModel CreateFields(Fixture fixture)
    {
        var fieldName = fixture.Create<string>();
        var fieldData = fixture.Create<string>();

        return new RecipeFieldModel(fieldName, fieldData);
    }
}