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
using System.Linq;
using AutoFixture;
using FluentAssertions;
using RecipeBot.Discord.Data;
using RecipeBot.Domain.Data;
using RecipeBot.Services;
using Xunit;

namespace RecipeBot.Test.Services;

public class RecipeDataBuilderTest
{
    [Theory]
    [InlineData(DiscordRecipeCategory.Dessert, RecipeCategory.Dessert)]
    [InlineData(DiscordRecipeCategory.Fish, RecipeCategory.Fish)]
    [InlineData(DiscordRecipeCategory.Meat, RecipeCategory.Meat)]
    [InlineData(DiscordRecipeCategory.Pastry, RecipeCategory.Pastry)]
    [InlineData(DiscordRecipeCategory.Snack, RecipeCategory.Snack)]
    [InlineData(DiscordRecipeCategory.Vegan, RecipeCategory.Vegan)]
    [InlineData(DiscordRecipeCategory.Vegetarian, RecipeCategory.Vegetarian)]
    [InlineData(DiscordRecipeCategory.Drinks, RecipeCategory.Drinks)]
    [InlineData(DiscordRecipeCategory.Other, RecipeCategory.Other)]
    public void Builder_with_recipe_category_builds_recipe_data_with_expected_category(
        DiscordRecipeCategory discordCategory, RecipeCategory expectedCategory)
    {
        // Setup
        var fixture = new Fixture();

        var authorId = fixture.Create<ulong>();
        var recipeTitle = fixture.Create<string>();
        var recipeIngredients = fixture.Create<string>();
        var cookingSteps = fixture.Create<string>();

        var builder = new RecipeDataBuilder(authorId, discordCategory, recipeTitle, recipeIngredients, cookingSteps);

        // Call
        RecipeData result = builder.Build();

        // Assert
        result.Category.Should().Be(expectedCategory);
    }

    [Fact]
    public void Builder_with_invalid_recipe_category_throws_exception()
    {
        // Setup
        var fixture = new Fixture();

        var authorId = fixture.Create<ulong>();
        var recipeTitle = fixture.Create<string>();
        var recipeIngredients = fixture.Create<string>();
        var cookingSteps = fixture.Create<string>();

        const DiscordRecipeCategory discordCategory = (DiscordRecipeCategory)(-1);

        // Call
        Action call = () => new RecipeDataBuilder(authorId, discordCategory, recipeTitle, recipeIngredients, cookingSteps);

        // Assert
        call.Should().Throw<InvalidEnumArgumentException>();
    }

    [Fact]
    public void Builder_without_optional_data_then_build_recipe_data_with_mandatory_fields()
    {
        // Setup
        var fixture = new Fixture();

        var authorId = fixture.Create<ulong>();
        var discordCategory = fixture.Create<DiscordRecipeCategory>();
        var recipeTitle = fixture.Create<string>();
        var recipeIngredients = fixture.Create<string>();
        var cookingSteps = fixture.Create<string>();

        var builder = new RecipeDataBuilder(authorId, discordCategory, recipeTitle, recipeIngredients, cookingSteps);

        // Call
        RecipeData result = builder.Build();

        // Assert
        result.RecipeFields.Should().HaveCount(2);
        AssertMandatoryRecipeFields(recipeTitle, recipeIngredients, cookingSteps, authorId, result);

        result.ImageUrl.Should().BeNull();
        result.Tags.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("    ")]
    [InlineData("")]
    public void Builder_when_adding_empty_notes_then_build_recipe_data_without_notes(string notes)
    {
        // Setup
        var fixture = new Fixture();

        var authorId = fixture.Create<ulong>();
        var discordCategory = fixture.Create<DiscordRecipeCategory>();
        var recipeTitle = fixture.Create<string>();
        var recipeIngredients = fixture.Create<string>();
        var cookingSteps = fixture.Create<string>();

        var builder = new RecipeDataBuilder(authorId, discordCategory, recipeTitle, recipeIngredients, cookingSteps);

        // Call
        RecipeData result = builder.AddNotes(notes)
                                   .Build();

        // Assert
        result.RecipeFields.Should().HaveCount(2);
        AssertMandatoryRecipeFields(recipeTitle, recipeIngredients, cookingSteps, authorId, result);
    }

    [Fact]
    public void Builder_when_adding_non_empty_notes_then_build_recipe_data_with_notes()
    {
        // Setup
        var fixture = new Fixture();

        var authorId = fixture.Create<ulong>();
        var discordCategory = fixture.Create<DiscordRecipeCategory>();
        var recipeTitle = fixture.Create<string>();
        var recipeIngredients = fixture.Create<string>();
        var cookingSteps = fixture.Create<string>();
        var notes = fixture.Create<string>();

        var builder = new RecipeDataBuilder(authorId, discordCategory, recipeTitle, recipeIngredients, cookingSteps);

        // Call
        RecipeData result = builder.AddNotes(notes)
                                   .Build();

        // Assert
        result.RecipeFields.Should().HaveCount(3);
        AssertMandatoryRecipeFields(recipeTitle, recipeIngredients, cookingSteps, authorId, result);

        RecipeFieldData notesField = result.RecipeFields.ElementAt(2);
        notesField.FieldName.Should().Be("Additional notes");
        notesField.FieldData.Should().Be(notes);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("    ")]
    [InlineData("")]
    [InlineData("tags")]
    public void Builder_when_adding_tags_then_build_recipe_data_with_tags(string tags)
    {
        // Setup
        var fixture = new Fixture();

        var authorId = fixture.Create<ulong>();
        var discordCategory = fixture.Create<DiscordRecipeCategory>();
        var recipeTitle = fixture.Create<string>();
        var recipeIngredients = fixture.Create<string>();
        var cookingSteps = fixture.Create<string>();

        var builder = new RecipeDataBuilder(authorId, discordCategory, recipeTitle, recipeIngredients, cookingSteps);

        // Call
        RecipeData result = builder.AddTags(tags)
                                   .Build();

        // Assert
        result.Tags.Should().Be(tags);
    }

    [Fact]
    public void Builder_with_optional_data_then_build_recipe_data_with_all_properties()
    {
        // Setup
        var fixture = new Fixture();

        var authorId = fixture.Create<ulong>();
        var discordCategory = fixture.Create<DiscordRecipeCategory>();
        var recipeTitle = fixture.Create<string>();
        var recipeIngredients = fixture.Create<string>();
        var cookingSteps = fixture.Create<string>();
        var notes = fixture.Create<string>();
        var tags = fixture.Create<string>();

        var builder = new RecipeDataBuilder(authorId, discordCategory, recipeTitle, recipeIngredients, cookingSteps);

        // Call
        RecipeData result = builder.AddNotes(notes)
                                   .AddTags(tags)
                                   .Build();

        // Assert
        result.RecipeFields.Should().HaveCount(3);
        AssertMandatoryRecipeFields(recipeTitle, recipeIngredients, cookingSteps, authorId, result);

        RecipeFieldData notesField = result.RecipeFields.ElementAt(2);
        notesField.FieldName.Should().Be("Additional notes");
        notesField.FieldData.Should().Be(notes);

        result.Tags.Should().Be(tags);
    }

    private static void AssertMandatoryRecipeFields(
        string expectedRecipeTitle, string expectedRecipeIngredients, string expectedCookingSteps, ulong expectedAuthorId,
        RecipeData actualRecipeData)
    {
        actualRecipeData.RecipeTitle.Should().Be(expectedRecipeTitle);
        actualRecipeData.AuthorId.Should().Be(expectedAuthorId);

        RecipeFieldData firstField = actualRecipeData.RecipeFields.ElementAt(0);
        firstField.FieldName.Should().Be("Ingredients");
        firstField.FieldData.Should().Be(expectedRecipeIngredients);

        RecipeFieldData secondField = actualRecipeData.RecipeFields.ElementAt(1);
        secondField.FieldName.Should().Be("Cooking steps");
        secondField.FieldData.Should().Be(expectedCookingSteps);
    }
}