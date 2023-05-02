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
using System.ComponentModel;
using Common.Utils;
using Discord;
using Discord.Common.Utils;
using RecipeBot.Discord.Data;
using RecipeBot.Domain.Data;
using RecipeBot.Properties;

namespace RecipeBot.Services;

/// <summary>
/// Builder to create instances of <see cref="RecipeData"/>.
/// </summary>
internal class RecipeDataBuilder
{
    private readonly ulong authorId;
    private readonly string recipeTitle;
    private string? imageUrl;
    private string? tags;

    private readonly RecipeCategory recipeCategory;
    private readonly List<RecipeFieldData> recipeFields;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeDataBuilder"/>.
    /// </summary>
    /// <param name="authorId">The id of the author.</param>
    /// <param name="category">The <see cref="DiscordRecipeCategory"/>.</param>
    /// <param name="recipeTitle">The title of the recipe.</param>
    /// <param name="recipeIngredients">The ingredients of the recipe.</param>
    /// <param name="cookingSteps">The cooking steps of the recipe.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="recipeTitle"/>, <paramref name="recipeIngredients"/>
    /// or <paramref name="cookingSteps"/> is <c>null</c> or consists of whitespaces.</exception>
    /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/> is invalid.</exception>
    public RecipeDataBuilder(ulong authorId, DiscordRecipeCategory category,
                             string recipeTitle, string recipeIngredients, string cookingSteps)
    {
        recipeTitle.IsNotNullOrWhiteSpaces(nameof(recipeTitle));

        this.authorId = authorId;
        this.recipeTitle = recipeTitle;
        recipeCategory = RecipeCategoryConverter.ConvertFrom(category);

        recipeFields = new List<RecipeFieldData>
        {
            new RecipeFieldData(Resources.RecipeFieldName_Ingredients_DisplayName, recipeIngredients),
            new RecipeFieldData(Resources.RecipeFieldName_Cooking_Steps_DisplayName, cookingSteps),
        };
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
            imageUrl = attachment.Url;
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
        if (!string.IsNullOrWhiteSpace(notes))
        {
            recipeFields.Add(new RecipeFieldData(Resources.RecipeFieldName_Additional_Notes_DisplayName, notes));
        }

        return this;
    }

    /// <summary>
    /// Adds tags to the <see cref="RecipeData"/>.
    /// </summary>
    /// <param name="tags">The tags to add.</param>
    /// <returns>The <see cref="RecipeDataBuilder"/>.</returns>
    public RecipeDataBuilder AddTags(string? tags)
    {
        this.tags = tags;
        return this;
    }

    /// <summary>
    /// Builds the <see cref="RecipeData"/>.
    /// </summary>
    /// <returns>A configured <see cref="RecipeData"/>.</returns>
    public RecipeData Build()
    {
        return new RecipeData(authorId, recipeFields, recipeTitle, recipeCategory)
        {
            Tags = tags,
            ImageUrl = imageUrl
        };
    }
}