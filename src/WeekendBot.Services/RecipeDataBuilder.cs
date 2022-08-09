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
using Discord.Common.Utils;
using WeekendBot.Utils;

namespace WeekendBot.Services;

/// <summary>
/// Builder to create instances of <see cref="RecipeData"/>.
/// </summary>
public class RecipeDataBuilder
{
    private readonly RecipeData data;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeDataBuilder"/>.
    /// </summary>
    /// <param name="authorData">The <see cref="AuthorData"/>.</param>
    /// <param name="recipeTitle">The title of the recipe.</param>
    /// <param name="recipeIngredients">The ingredients of the recipe.</param>
    /// <param name="cookingSteps">The cooking steps of the recipe.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="authorData"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="recipeTitle"/>, <see cref="recipeIngredients"/>
    /// or <paramref name="cookingSteps"/> is <c>null</c> or consists of whitespaces.</exception>
    public RecipeDataBuilder(AuthorData authorData, string? recipeTitle, string? recipeIngredients, string? cookingSteps)
    {
        data = new RecipeData(authorData, recipeTitle, recipeIngredients, cookingSteps);
    }

    /// <summary>
    /// Adds an attachment image url to the <see cref="RecipeData"/>.
    /// </summary>
    /// <param name="attachment">The <see cref="IAttachment"/> to add the information from.</param>
    /// <returns>The <see cref="RecipeDataBuilder"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when the attachment image url could not be added.</exception>
    public RecipeDataBuilder AddImage(IAttachment? attachment)
    {
        if (attachment != null)
        {
            attachment.IsValidArgument(x => x.IsImage(), "Attachment must be an image.", nameof(attachment));
            data.ImageUrl = attachment.Url;
        }

        return this;
    }

    /// <summary>
    /// Adds notes to the <see cref="RecipeData"/>.
    /// </summary>
    /// <param name="notes">The notes to add.</param>
    /// <returns>The <see cref="RecipeDataBuilder"/>.</returns>
    public RecipeDataBuilder AddNotes(string? notes)
    {
        data.AdditionalNotes = notes;
        return this;
    }

    /// <summary>
    /// Builds the <see cref="RecipeData"/>.
    /// </summary>
    /// <returns>A configured <see cref="RecipeData"/>.</returns>
    public RecipeData Build()
    {
        return data;
    }
}