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
using System.Linq;
using Common.Utils;
using RecipeBot.Domain.Data;

namespace RecipeBot.Domain.Models;

/// <summary>
/// Model containing data for a recipe.
/// </summary>
public class RecipeModel
{
    /// <summary>
    /// Creates a new instance of <see cref="RecipeModel"/>.
    /// </summary>
    /// <param name="metaData">The metadata of the recipe.</param>
    /// <param name="recipeFields">The collection of recipe fields.</param>
    /// <param name="title">The title of the recipe.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter, except <paramref name="title"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="title"/> is <c>null</c>, empty or consists
    /// of whitespaces.</exception>
    internal RecipeModel(RecipeModelMetaData metaData, IEnumerable<RecipeFieldModel> recipeFields, string title)
    {
        metaData.IsNotNull(nameof(metaData));
        recipeFields.IsNotNull(nameof(recipeFields));
        title.IsNotNullOrWhiteSpaces(nameof(title));

        Author = metaData.Author;
        RecipeCategory = metaData.Category;
        RecipeFields = recipeFields;
        RecipeTags = metaData.Tags;
        Title = title;
    }

    /// <summary>
    /// Creates a new instance of <see cref="RecipeModel"/>.
    /// </summary>
    /// <param name="metaData">The metadata of the recipe.</param>
    /// <param name="recipeFields">The collection of recipe fields.</param>
    /// <param name="title">The title of the recipe.</param>
    /// <param name="recipeImageUrl">The image url of the recipe.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter except <paramref name="title"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when:
    /// <list type="bullet">
    /// <item><paramref name="title"/> is <c>null</c>, empty or consists of whitespaces; or</item>
    /// <item><paramref name="recipeImageUrl"/> is an invalid url.</item>
    /// </list>
    /// </exception>
    internal RecipeModel(RecipeModelMetaData metaData, IEnumerable<RecipeFieldModel> recipeFields, string title, string recipeImageUrl)
        : this(metaData, recipeFields, title)
    {
        UrlValidationHelper.ValidateHttpUrl(recipeImageUrl);
        RecipeImageUrl = recipeImageUrl;
    }

    /// <summary>
    /// Gets the title of the recipe.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the image url of the recipe.
    /// </summary>
    public string? RecipeImageUrl { get; }

    /// <summary>
    /// Gets the author information.
    /// </summary>
    public AuthorModel Author { get; }

    /// <summary>
    /// Gets the category the recipe is based on.
    /// </summary>
    public RecipeCategory RecipeCategory { get; }

    /// <summary>
    /// Gets the fields the recipe consists of.
    /// </summary>
    public IEnumerable<RecipeFieldModel> RecipeFields { get; }

    /// <summary>
    /// Gets the tags of the recipe.
    /// </summary>
    public RecipeTagsModel RecipeTags { get; }

    /// <summary>
    /// Gets the total character length of the model.
    /// </summary>
    public int TotalLength
    {
        get
        {
            return Title.Length + Author.TotalLength + RecipeFields.Sum(f => f.TotalLength) + RecipeTags.TotalLength;
        }
    }
}