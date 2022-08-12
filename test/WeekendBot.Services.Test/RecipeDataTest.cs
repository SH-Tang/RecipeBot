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
using WeekendBot.TestUtils;
using Xunit;

namespace WeekendBot.Services.Test;

public class RecipeDataTest
{
    [Theory]
    [ClassData(typeof(EmptyOrNullStringValueGenerator))]
    public void Given_recipe_data_with_invalid_recipe_title_throws_exception(string invalidRecipeTitle)
    {
        // Setup
        const string recipeIngredients = "My ingredients";
        const string cookingSteps = "My recipe steps";

        // Call
        Func<RecipeData> call = () => new RecipeData(CreateValidAuthorData(), invalidRecipeTitle, recipeIngredients, cookingSteps);

        // Assert
        Assert.Throws<ArgumentException>(call);
    }

    [Theory]
    [ClassData(typeof(EmptyOrNullStringValueGenerator))]
    public void Given_recipe_data_with_invalid_recipe_ingredients_throws_exception(string invalidRecipeIngredients)
    {
        // Setup
        const string recipeTitle = "Recipe title";
        const string cookingSteps = "My recipe steps";

        // Call
        Func<RecipeData> call = () => new RecipeData(CreateValidAuthorData(), recipeTitle, invalidRecipeIngredients, cookingSteps);

        // Assert
        Assert.Throws<ArgumentException>(call);
    }

    [Theory]
    [ClassData(typeof(EmptyOrNullStringValueGenerator))]
    public void Given_recipe_data_with_invalid_cooking_steps_throws_exception(string invalidCookingSteps)
    {
        // Setup
        const string recipeTitle = "Recipe title";
        const string recipeIngredients = "My ingredients";

        // Call
        Func<RecipeData> call = () => new RecipeData(CreateValidAuthorData(), recipeTitle, recipeIngredients, invalidCookingSteps);

        // Assert
        Assert.Throws<ArgumentException>(call);
    }

    private static AuthorData CreateValidAuthorData()
    {
        const string authorName = "Recipe author";
        const string authorImageUrl = "https://AuthorImage.url";
        return new AuthorData(authorName, authorImageUrl);
    }
}