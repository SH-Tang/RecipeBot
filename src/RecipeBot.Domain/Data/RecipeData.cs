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

namespace RecipeBot.Domain.Data;

/// <summary>
/// Class containing information about a recipe.
/// </summary>
public class RecipeData
{
    /// <summary>
    /// Creates a new instance of <see cref="RecipeData"/>.
    /// </summary>
    /// <param name="authorData">The <see cref="Data.AuthorData"/>.</param>
    /// <param name="recipeFields">The collection of fields to contain within the recipe.</param>
    /// <param name="recipeTitle">The title of the recipe.</param>
    /// <param name="category">The <see cref="RecipeCategory"/> the recipe belongs to.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="authorData"/> or <paramref name="recipeFields"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="recipeTitle"/> is <c>null</c> or consists of whitespaces.</exception>
    /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/> is an invalid <see cref="RecipeCategory"/>.</exception>
    public RecipeData(AuthorData authorData, IEnumerable<RecipeFieldData> recipeFields, string recipeTitle, RecipeCategory category)
    {
        authorData.IsNotNull(nameof(authorData));
        recipeFields.IsNotNull(nameof(recipeFields));
        recipeTitle.IsNotNullOrWhiteSpaces(nameof(recipeTitle));
        category.IsValidEnum(nameof(category));

        AuthorData = authorData;
        RecipeFields = recipeFields;
        RecipeTitle = recipeTitle;
        Category = category;
    }

    /// <summary>
    /// Gets the <see cref="Data.AuthorData"/>.
    /// </summary>
    public AuthorData AuthorData { get; }

    /// <summary>
    /// Gets the <see cref="RecipeCategory"/>.
    /// </summary>
    public RecipeCategory Category { get; }

    /// <summary>
    /// Gets the collection of <see cref="RecipeFieldData"/>.
    /// </summary>
    public IEnumerable<RecipeFieldData> RecipeFields { get; }

    /// <summary>
    /// Gets the title of the recipe.
    /// </summary>
    public string RecipeTitle { get; }

    /// <summary>
    /// Gets the ingredients of the recipe.
    /// </summary>
    public string RecipeIngredients { get; }

    /// <summary>
    /// Gets the cooking steps of the recipe.
    /// </summary>
    public string CookingSteps { get; }

    /// <summary>
    /// Gets or sets the additional notes of the recipe.
    /// </summary>
    public string? AdditionalNotes { get; set; }

    /// <summary>
    /// Gets or sets the image url of the recipe.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the tags of the recipe. 
    /// </summary>
    public string? Tags { get; set; }
}