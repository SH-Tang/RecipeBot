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
using AutoFixture;
using FluentAssertions;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Models;
using RecipeBot.Domain.TestUtils;
using RecipeBot.Persistence.Creators;
using RecipeBot.Persistence.Entities;
using Xunit;

namespace RecipeBot.Persistence.Test.Creators;

public class RecipeEntityCreatorTest
{
    [Theory]
    [InlineData(RecipeCategory.Dessert, PersistentRecipeCategory.Dessert)]
    [InlineData(RecipeCategory.Fish, PersistentRecipeCategory.Fish)]
    [InlineData(RecipeCategory.Meat, PersistentRecipeCategory.Meat)]
    [InlineData(RecipeCategory.Pastry, PersistentRecipeCategory.Pastry)]
    [InlineData(RecipeCategory.Snack, PersistentRecipeCategory.Snack)]
    [InlineData(RecipeCategory.Vegan, PersistentRecipeCategory.Vegan)]
    [InlineData(RecipeCategory.Vegetarian, PersistentRecipeCategory.Vegetarian)]
    [InlineData(RecipeCategory.Drinks, PersistentRecipeCategory.Drinks)]
    [InlineData(RecipeCategory.Other, PersistentRecipeCategory.Other)]
    public void Given_model_with_category_when_creating_entity_returns_entity_with_expected_category(
        RecipeCategory category, PersistentRecipeCategory expectedCategory)
    {
        // Setup
        var fixture = new Fixture();
        var authorEntity = new AuthorEntity
        {
            AuthorName = fixture.Create<string>(),
            AuthorImageUrl = fixture.Create<string>()
        };

        var testBuilder = new RecipeModelTestBuilder();
        RecipeModel recipeModel = testBuilder.SetCategory(category)
                                             .Build();

        // Call
        RecipeEntity recipeEntity = RecipeEntityCreator.Create(recipeModel, authorEntity, new List<RecipeTagEntity>());

        // Assert
        recipeEntity.RecipeCategory.Should().Be(expectedCategory);
    }
}