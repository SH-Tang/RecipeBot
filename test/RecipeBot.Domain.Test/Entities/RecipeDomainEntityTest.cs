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
using WeekendBot.Domain.Entities;
using WeekendBot.TestUtils;
using Xunit;

namespace WeekendBot.Domain.Test.Entities;

public class RecipeDomainEntityTest
{
    private const string imageUrl = "http://just.an.image";

    [Theory]
    [ClassData(typeof(NullOrWhitespacesStringValueGenerator))]
    public void Entity_with_invalid_recipe_title_throws_exception(string invalidRecipeTitle)
    {
        // Setup
        AuthorDomainEntity authorEntity = CreateAuthorEntity();

        // Call
        Action call = () => new RecipeDomainEntity(authorEntity, Enumerable.Empty<RecipeFieldDomainEntity>(),
                                                   invalidRecipeTitle);

        // Assert
        Assert.Throws<ArgumentException>(call);
    }

    [Theory]
    [ClassData(typeof(NullOrWhitespacesStringValueGenerator))]
    public void Entity_with_image_url_and_invalid_title_throws_exception(string invalidRecipeTitle)
    {
        // Setup
        AuthorDomainEntity authorEntity = CreateAuthorEntity();

        // Call
        Action call = () => new RecipeDomainEntity(authorEntity, Enumerable.Empty<RecipeFieldDomainEntity>(),
                                                   invalidRecipeTitle, imageUrl);

        // Assert
        Assert.Throws<ArgumentException>(call);
    }

    [Fact]
    public void Entity_without_image_url_sets_image_url_null()
    {
        // Setup
        AuthorDomainEntity authorEntity = CreateAuthorEntity();

        // Call
        var recipe = new RecipeDomainEntity(authorEntity, Enumerable.Empty<RecipeFieldDomainEntity>(), "recipeTitle");

        // Assert
        Assert.Null(recipe.RecipeImageUrl);
    }

    [Fact]
    public void Entity_with_valid_image_url_sets_image_url()
    {
        // Setup
        AuthorDomainEntity authorEntity = CreateAuthorEntity();

        // Call
        var recipe = new RecipeDomainEntity(authorEntity, Enumerable.Empty<RecipeFieldDomainEntity>(), "recipeTitle", imageUrl);

        // Assert
        Assert.Equal(imageUrl, recipe.RecipeImageUrl);
    }

    [Theory]
    [ClassData(typeof(InvalidHttpUrlDataGenerator))]
    public void Entity_with_invalid_image_url_throws_exception(string invalidImageUrl)
    {
        // Setup
        AuthorDomainEntity authorEntity = CreateAuthorEntity();

        // Call
        Action call = () => new RecipeDomainEntity(authorEntity, Enumerable.Empty<RecipeFieldDomainEntity>(), "recipeTitle", invalidImageUrl);

        // Assert
        Assert.Throws<ArgumentException>(call);
    }

    [Theory]
    [InlineData("recipeTitle")]
    [InlineData("recipe title")]
    [InlineData("recipe    title")]
    [InlineData("     recipeTitle")]
    [InlineData("recipeTitle     ")]
    public void Entity_with_valid_title_returns_total_length_of_properties(string recipeTitle)
    {
        // Setup
        var random = new Random(21);

        AuthorDomainEntity authorEntity = CreateAuthorEntity();
        IEnumerable<RecipeFieldDomainEntity> recipeFieldEntities = new[]
        {
            CreateFieldEntity(random.Next()),
            CreateFieldEntity(random.Next()),
            CreateFieldEntity(random.Next())
        };

        var recipe = new RecipeDomainEntity(authorEntity, recipeFieldEntities, recipeTitle);

        // Call
        int totalLength = recipe.TotalLength;

        // Assert
        int expectedLength = recipeTitle.Length + authorEntity.TotalLength + recipeFieldEntities.Sum(f => f.TotalLength);
        Assert.Equal(expectedLength, totalLength);
    }

    [Theory]
    [InlineData("recipeTitle")]
    [InlineData("recipe title")]
    [InlineData("recipe    title")]
    [InlineData("     recipeTitle")]
    [InlineData("recipeTitle     ")]
    public void Entity_with_valid_title_and_image_url_returns_total_length_of_properties(string recipeTitle)
    {
        // Setup
        var random = new Random(21);

        AuthorDomainEntity authorEntity = CreateAuthorEntity();
        IEnumerable<RecipeFieldDomainEntity> recipeFieldEntities = new[]
        {
            CreateFieldEntity(random.Next()),
            CreateFieldEntity(random.Next()),
            CreateFieldEntity(random.Next())
        };

        var recipe = new RecipeDomainEntity(authorEntity, recipeFieldEntities, recipeTitle, imageUrl);

        // Call
        int totalLength = recipe.TotalLength;

        // Assert
        int expectedLength = recipeTitle.Length + authorEntity.TotalLength + recipeFieldEntities.Sum(f => f.TotalLength);
        Assert.Equal(expectedLength, totalLength);
    }

    [Fact]
    public void Entity_with_empty_recipe_fields_returns_total_length_of_properties()
    {
        // Setup
        const string recipeTitle = "recipeTitle";

        AuthorDomainEntity authorEntity = CreateAuthorEntity();
        var recipe = new RecipeDomainEntity(authorEntity, Enumerable.Empty<RecipeFieldDomainEntity>(), recipeTitle);

        // Call
        int totalLength = recipe.TotalLength;

        // Assert
        int expectedLength = recipeTitle.Length + authorEntity.TotalLength;
        Assert.Equal(expectedLength, totalLength);
    }

    [Fact]
    public void Entity_with_image_url_and_empty_recipe_fields_returns_total_length_of_properties()
    {
        // Setup
        const string recipeTitle = "recipeTitle";

        AuthorDomainEntity authorEntity = CreateAuthorEntity();
        var recipe = new RecipeDomainEntity(authorEntity, Enumerable.Empty<RecipeFieldDomainEntity>(), recipeTitle, imageUrl);

        // Call
        int totalLength = recipe.TotalLength;

        // Assert
        int expectedLength = recipeTitle.Length + authorEntity.TotalLength;
        Assert.Equal(expectedLength, totalLength);
    }

    private static AuthorDomainEntity CreateAuthorEntity()
    {
        const string authorImageUrl = "http://www.google.com";

        var random = new Random(21);
        var authorName = new string('o', random.Next(100));

        return new AuthorDomainEntity(authorName, authorImageUrl);
    }

    private static RecipeFieldDomainEntity CreateFieldEntity(int seed)
    {
        var random = new Random(seed);
        var fieldName = new string('+', random.Next(100));
        var fieldData = new string('x', random.Next(100));

        return new RecipeFieldDomainEntity(fieldName, fieldData);
    }
}