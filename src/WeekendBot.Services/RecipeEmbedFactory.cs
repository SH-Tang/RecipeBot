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
using Discord;
using WeekendBot.Services.Properties;
using WeekendBot.Utils;

namespace WeekendBot.Services;

/// <summary>
/// Factory to create instances of <see cref="Embed"/> that contains recipe data.
/// </summary>
public static class RecipeEmbedFactory
{
    /// <summary>
    /// Creates an <see cref="Embed"/> based on its input arguments.
    /// </summary>
    /// <param name="recipeData">The <see cref="RecipeData"/> to create the <see cref="Embed"/> with.</param>
    /// <returns>A <see cref="Embed"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="recipeData"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="recipeData"/> contains invalid values.</exception>
    /// <exception cref="ModalResponseException">Thrown when the response could not be successfully determined.</exception>
    public static Embed Create(RecipeData recipeData)
    {
        recipeData.IsNotNull(nameof(recipeData));

        try
        {
            EmbedBuilder embedBuilder = CreateConfiguredEmbedBuilder(recipeData);
            return embedBuilder.Build();
        }
        catch (InvalidOperationException e)
        {
            throw new ModalResponseException(string.Format(Resources.RecipeModal_response_could_not_be_determined_reason_0_, e.Message), e);
        }
    }

    private static EmbedBuilder CreateConfiguredEmbedBuilder(RecipeData recipeData)
    {
        AuthorData authorData = recipeData.AuthorData;
        EmbedBuilder embedBuilder = new EmbedBuilder().WithAuthor(authorData.AuthorName, authorData.AuthorImageUrl)
                                                      .WithTitle(recipeData.RecipeTitle);
        if (!string.IsNullOrWhiteSpace(recipeData.ImageUrl))
        {
            embedBuilder.WithImageUrl(recipeData.ImageUrl);
        }

        ConfigureFields(recipeData, embedBuilder);
        
        return embedBuilder;
    }

    private static void ConfigureFields(RecipeData recipeData, EmbedBuilder embedBuilder)
    {
        try
        {
            embedBuilder.AddField("Ingredients", recipeData.RecipeIngredients)
                        .AddField("Cooking steps", recipeData.CookingSteps);

            if (!string.IsNullOrWhiteSpace(recipeData.AdditionalNotes))
            {
                embedBuilder.AddField("Additional notes", recipeData.AdditionalNotes);
            }
        }
        catch (ArgumentException e)
        {
            throw new ModalResponseException(string.Format(Resources.RecipeModal_response_could_not_be_determined_reason_0_, e.Message), e);
        }
    }
}