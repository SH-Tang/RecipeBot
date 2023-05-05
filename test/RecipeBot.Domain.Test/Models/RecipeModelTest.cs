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
using FluentAssertions;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Models;
using RecipeBot.Domain.TestUtils;
using RecipeBot.TestUtils;
using Xunit;

namespace RecipeBot.Domain.Test.Models;

public class RecipeModelTest
{
    [Theory]
    [ClassData(typeof(NullOrWhitespacesStringValueGenerator))]
    public void Model_with_invalid_recipe_title_throws_exception(string invalidRecipeTitle)
    {
        // Setup
        var fixture = new Fixture();

        // Call
        Action call = () => new RecipeModel(CreateMetaData(fixture), Enumerable.Empty<RecipeFieldModel>(), invalidRecipeTitle);

        // Assert
        call.Should().ThrowExactly<ArgumentException>();
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
        fixture.Register<string, RecipeFieldModel>(x => new RecipeFieldModel(x, x));

        IEnumerable<RecipeFieldModel> recipeFields = fixture.CreateMany<RecipeFieldModel>();
        RecipeModelMetaData metaData = CreateMetaData(fixture);
        RecipeModel recipe = fixture.Build<RecipeModel>()
                                    .FromFactory(() => new RecipeModel(metaData, recipeFields, recipeTitle))
                                    .Create();

        // Call
        int totalLength = recipe.TotalLength;

        // Assert
        int expectedLength = recipeTitle.Length + recipeFields.Sum(f => f.TotalLength)
                                                + TagTestHelper.GetTotalTagsLength(metaData.Category, metaData.Tags);
        totalLength.Should().Be(expectedLength);
    }

    [Fact]
    public void Model_with_empty_recipe_fields_returns_total_length_of_properties()
    {
        // Setup
        var fixture = new Fixture();
        var recipeTitle = fixture.Create<string>();

        RecipeModelMetaData metaData = CreateMetaData(fixture);
        RecipeModel recipe = fixture.Build<RecipeModel>()
                                    .FromFactory(() => new RecipeModel(metaData, Enumerable.Empty<RecipeFieldModel>(), recipeTitle))
                                    .Create();

        // Call
        int totalLength = recipe.TotalLength;

        // Assert
        int expectedLength = recipeTitle.Length + recipe.RecipeTags.TotalLength;
        totalLength.Should().Be(expectedLength);
    }

    private static RecipeModelMetaData CreateMetaData(Fixture fixture)
    {
        var authorId = fixture.Create<ulong>();
        var category = fixture.Create<RecipeCategory>();

        IEnumerable<string> tags = fixture.CreateMany<string>();
        RecipeTagsModel tagsModel = fixture.Build<RecipeTagsModel>()
                                           .FromFactory(() => new RecipeTagsModel(tags))
                                           .Create();

        return new RecipeModelMetaData(authorId, tagsModel, category);
    }
}