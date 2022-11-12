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
using Common.Utils;
using RecipeBot.Domain.Data;

namespace RecipeBot.Domain.Models;

/// <summary>
/// Class containing the metadata of a recipe.
/// </summary>
internal class RecipeModelMetaData
{
    /// <summary>
    /// Creates a new instance of a <see cref="RecipeModelMetaData"/>.
    /// </summary>
    /// <param name="author">The author information/</param>
    /// <param name="tags">The tags.</param>
    /// <param name="category">The <see cref="RecipeCategory"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="author"/> or <paramref name="tags"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/> is an invalid <see cref="RecipeCategory"/>.</exception>
    public RecipeModelMetaData(AuthorModel author, RecipeTagsModel tags, RecipeCategory category)
    {
        author.IsNotNull(nameof(author));
        tags.IsNotNull(nameof(tags));
        category.IsValidEnum(nameof(category));

        Author = author;
        Tags = tags;
        Category = category;
    }

    /// <summary>
    /// Gets the author information.
    /// </summary>
    public AuthorModel Author { get; }

    /// <summary>
    /// Gets the tags belonging to the recipe.
    /// </summary>
    public RecipeTagsModel Tags { get; }

    /// <summary>
    /// Gets the category of the recipe.
    /// </summary>
    public RecipeCategory Category { get; }
}