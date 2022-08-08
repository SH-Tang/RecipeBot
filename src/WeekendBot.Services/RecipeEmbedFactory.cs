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

using Discord;
using WeekendBot.Utils;

namespace WeekendBot.Services;

/// <summary>
/// Factory to create instances of <see cref="Embed"/> that contains recipe data.
/// </summary>
public static class RecipeEmbedFactory
{
    public static Embed Create(RecipeData recipeData)
    {
        recipeData.IsNotNull(nameof(recipeData));

        EmbedBuilder? embedBuilder = new EmbedBuilder()
                                     .WithAuthor(recipeData.AuthorData.AuthorName, recipeData.AuthorData.AuthorImageUrl)
                                     .WithTitle(recipeData.RecipeTitle)
                                     .AddField("Ingredients", recipeData.RecipeIngredients)
                                     .AddField("Cooking steps", recipeData.CookingSteps);

        if (!string.IsNullOrWhiteSpace(recipeData.AdditionalNotes))
        {
            embedBuilder.AddField("Additional notes", recipeData.AdditionalNotes);
        }

        if (!string.IsNullOrWhiteSpace(recipeData.ImageUrl))
        {
            embedBuilder.WithImageUrl(recipeData.ImageUrl);
        }

        return embedBuilder.Build();
    }
}