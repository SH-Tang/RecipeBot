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
using System.ComponentModel;
using AutoFixture;
using RecipeBot.Domain.Data;
using RecipeBot.TestUtils;
using Xunit;

namespace RecipeBot.Domain.Test.Data;

public class RecipeDataTest
{
    [Fact]
    public void Given_recipe_data_with_invalid_recipe_category_throws_exception()
    {
        // Setup
        var fixture = new Fixture();
        AuthorData authorData = CreateValidAuthorData(fixture);

        const RecipeCategory category = (RecipeCategory) (-1);


        // Call
        Action call = () => new RecipeData(authorData, category, fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>());

        // Assert
        Assert.Throws<InvalidEnumArgumentException>(call);
    }

    [Theory]
    [ClassData(typeof(NullOrWhitespacesStringValueGenerator))]
    public void Given_recipe_data_with_invalid_recipe_title_throws_exception(string invalidRecipeTitle)
    {
        // Setup
        var fixture = new Fixture();
        AuthorData authorData = CreateValidAuthorData(fixture);

        // Call
        Action call = () => new RecipeData(authorData, fixture.Create<RecipeCategory>(), invalidRecipeTitle, fixture.Create<string>(), fixture.Create<string>());

        // Assert
        Assert.Throws<ArgumentException>(call);
    }

    [Theory]
    [ClassData(typeof(NullOrWhitespacesStringValueGenerator))]
    public void Given_recipe_data_with_invalid_recipe_ingredients_throws_exception(string invalidRecipeIngredients)
    {
        // Setup
        var fixture = new Fixture();
        AuthorData authorData = CreateValidAuthorData(fixture);

        // Call
        Action call = () => new RecipeData(authorData, fixture.Create<RecipeCategory>(), fixture.Create<string>(), invalidRecipeIngredients, fixture.Create<string>());

        // Assert
        Assert.Throws<ArgumentException>(call);
    }

    [Theory]
    [ClassData(typeof(NullOrWhitespacesStringValueGenerator))]
    public void Given_recipe_data_with_invalid_cooking_steps_throws_exception(string invalidCookingSteps)
    {
        // Setup
        var fixture = new Fixture();
        AuthorData authorData = CreateValidAuthorData(fixture);

        // Call
        Action call = () => new RecipeData(authorData, fixture.Create<RecipeCategory>(), fixture.Create<string>(), fixture.Create<string>(), invalidCookingSteps);

        // Assert
        Assert.Throws<ArgumentException>(call);
    }

    private static AuthorData CreateValidAuthorData(Fixture fixture)
    {
        return fixture.Build<AuthorData>()
                      .FromFactory<string>(name => new AuthorData(name, "http://www.recipeBotImage.com"))
                      .Create();
    }
}