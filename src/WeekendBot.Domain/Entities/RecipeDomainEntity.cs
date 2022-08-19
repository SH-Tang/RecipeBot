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
using System.Collections.Generic;
using System.Linq;
using WeekendBot.Utils;

namespace WeekendBot.Domain.Entities;

/// <summary>
/// Entity containing data for a recipe.
/// </summary>
public class RecipeDomainEntity : ITotalCharacterLength
{
    /// <summary>
    /// Creates a new instance of <see cref="RecipeDomainEntity"/>.
    /// </summary>
    /// <param name="authorEntity">The author entity.</param>
    /// <param name="recipeFieldEntities">The collection of recipe field entities.</param>
    /// <param name="title">The title of the recipe.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter except <paramref name="title"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="title"/> is <c>null</c>, empty or consists of whitespaces.</exception>
    internal RecipeDomainEntity(AuthorDomainEntity authorEntity, IEnumerable<RecipeFieldDomainEntity> recipeFieldEntities, string title)
    {
        authorEntity.IsNotNull(nameof(authorEntity));
        recipeFieldEntities.IsNotNull(nameof(recipeFieldEntities));
        title.IsNotNullOrWhiteSpaces(nameof(title));

        AuthorEntity = authorEntity;
        RecipeFieldEntities = recipeFieldEntities;
        Title = title;
    }

    /// <summary>
    /// Gets the title of the recipe.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the author information.
    /// </summary>
    public AuthorDomainEntity AuthorEntity { get; }

    /// <summary>
    /// Gets the fields the recipe consists of.
    /// </summary>
    public IEnumerable<RecipeFieldDomainEntity> RecipeFieldEntities { get; }

    public int TotalLength
    {
        get
        {
            return Title.Length + AuthorEntity.TotalLength + RecipeFieldEntities.Sum(f => f.TotalLength);
        }
    }
}