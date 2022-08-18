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
using System.Linq;
using WeekendBot.Domain.Entities;
using WeekendBot.TestUtils;
using Xunit;

namespace WeekendBot.Domain.Test.Entities;

public class RecipeDomainEntityTest
{
    [Theory]
    [ClassData(typeof(EmptyOrNullStringValueGenerator))]
    public void Entity_with_invalid_recipe_title_throws_exception(string invalidRecipeTitle)
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

        // Call
        Func<RecipeDomainEntity> call = () => new RecipeDomainEntity(authorEntity, recipeFieldEntities, invalidRecipeTitle);

        // Assert
        Assert.Throws<ArgumentException>(call);
    }

    [Theory]
    [InlineData("recipeTitle")]
    [InlineData("recipe title")]
    [InlineData("recipe    title")]
    [InlineData("     recipeTitle")]
    [InlineData("recipeTitle     ")]
    public void Entity_with_valid_data_returns_total_length_of_properties(string recipeTitle)
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