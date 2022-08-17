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
using WeekendBot.Utils;

namespace WeekendBot.Domain;

/// <summary>
/// Class containing information about a recipe.
/// </summary>
public class RecipeData
{
    /// <summary>
    /// Creates a new instance of <see cref="RecipeData"/>.
    /// </summary>
    /// <param name="authorData">The <see cref="Domain.AuthorData"/>.</param>
    /// <param name="recipeTitle">The title of the recipe.</param>
    /// <param name="recipeIngredients">The ingredients of the recipe.</param>
    /// <param name="cookingSteps">The cooking steps of the recipe.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="authorData"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="recipeTitle"/>, <paramref name="recipeIngredients"/>
    /// or <paramref name="cookingSteps"/> is <c>null</c> or consists of whitespaces.</exception>
    public RecipeData(AuthorData authorData, string recipeTitle, string recipeIngredients, string cookingSteps)
    {
        authorData.IsNotNull(nameof(authorData));
        recipeTitle.IsNotNullOrWhiteSpaces(nameof(recipeTitle));
        recipeIngredients.IsNotNullOrWhiteSpaces(nameof(recipeIngredients));
        cookingSteps.IsNotNullOrWhiteSpaces(nameof(cookingSteps));

        AuthorData = authorData;
        RecipeTitle = recipeTitle;
        RecipeIngredients = recipeIngredients;
        CookingSteps = cookingSteps;
    }

    /// <summary>
    /// Gets the <see cref="Domain.AuthorData"/>.
    /// </summary>
    public AuthorData AuthorData { get; }

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
    public string? ImageUrl { get;  set; }
}