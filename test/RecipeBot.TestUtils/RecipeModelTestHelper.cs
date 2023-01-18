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
using FluentAssertions;
using RecipeBot.Discord.Data;
using RecipeBot.Discord.Views;
using RecipeBot.Domain.Models;
using RecipeBot.Domain.TestUtils;

namespace RecipeBot.TestUtils;

/// <summary>
/// Class that contains helper methods to assert instances of <see cref="RecipeModel"/>.
/// </summary>
public static class RecipeModelTestHelper
{
    /// <summary>
    /// Asserts a <see cref="RecipeModel"/> against data.
    /// </summary>
    /// <param name="user">The expected <see cref="IUser"/> data.</param>
    /// <param name="category">The expected <see cref="DiscordRecipeCategory"/>.</param>
    /// <param name="modal">The expected <see cref="RecipeModal"/>.</param>
    /// <param name="actualRecipe">The <see cref="RecipeModel"/> to assert.</param>
    public static void AssertCommonModelProperties(IUser user, DiscordRecipeCategory category,
                                                   RecipeModal modal, RecipeModel actualRecipe)
    {
        actualRecipe.Title.Should().Be(modal.RecipeTitle);
        actualRecipe.RecipeCategory.Should().Be(DiscordRecipeCategoryTestHelper.RecipeCategoryMapping[category]);

        string? expectedAvatarUrl = user.GetAvatarUrl();
        actualRecipe.Author.Should().Match<AuthorModel>(s => s.AuthorName == user.Username && s.AuthorImageUrl == expectedAvatarUrl);

        actualRecipe.RecipeFields.Should().SatisfyRespectively(
            firstField =>
            {
                firstField.FieldName.Should().Be("Ingredients");
                firstField.FieldData.Should().Be(modal.Ingredients);
            },
            secondField =>
            {
                secondField.FieldName.Should().Be("Cooking steps");
                secondField.FieldData.Should().Be(modal.CookingSteps);
            },
            thirdField =>
            {
                thirdField.FieldName.Should().Be("Additional notes");
                thirdField.FieldData.Should().Be(modal.Notes);
            });

        AssertTags(modal.Tags, actualRecipe.RecipeTags);
    }

    private static void AssertTags(string? tags, RecipeTagsModelWrapper actualTags)
    {
        if (tags != null)
        {
            IEnumerable<string> expectedTags = TagTestHelper.GetParsedTags(tags);
            actualTags.Tags.Should().BeEquivalentTo(expectedTags);
        }
        else
        {
            actualTags.Tags.Should().BeEmpty();
        }
    }
}