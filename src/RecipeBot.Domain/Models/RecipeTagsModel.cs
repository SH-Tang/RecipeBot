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
using Common.Utils;

namespace RecipeBot.Domain.Models;

/// <summary>
/// Model containing information about the recipe tags.
/// </summary>
public class RecipeTagsModel
{
    /// <summary>
    /// Creates a new instance of <see cref="RecipeTagsModel"/>.
    /// </summary>
    /// <param name="tags">The collection of tags.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tags"/> is <c>null</c>.</exception>
    internal RecipeTagsModel(IEnumerable<string> tags)
    {
        tags.IsNotNull(nameof(tags));
        Tags = tags;
    }

    /// <summary>
    /// Gets the collection of tags.
    /// </summary>
    public IEnumerable<string> Tags { get; }

    /// <summary>
    /// Gets the total character length of the model.
    /// </summary>
    public int TotalLength => ToString().Length;

    public override string ToString()
    {
        return string.Join(", ", Tags);
    }
}