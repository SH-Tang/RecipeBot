﻿// Copyright (C) 2022 Dennis Tang. All rights reserved.
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

using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Repositories.Data;

namespace RecipeBot.Domain.Repositories;

/// <summary>
/// Interface for describing a repository for handling data persistence of recipe collections.
/// </summary>
public interface IRecipeCollectionRepository
{
    /// <summary>
    /// Gets all the recipes.
    /// </summary>
    /// <returns>A collection of recipe entries.</returns>
    /// <exception cref="RepositoryDataLoadException">Thrown when the entries could not be successfully loaded.</exception>
    Task<IReadOnlyList<RecipeRepositoryEntityData>> LoadRecipeEntriesAsync();

    /// <summary>
    /// Gets all the recipes filtered by a <see cref="RecipeCategory"/>.
    /// </summary>
    /// <param name="category">The <see cref="RecipeCategory"/> to filter the recipes with.</param>
    /// <returns>A collection of filtered recipe entries.</returns>
    /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/> is an invalid <see cref="RecipeCategory"/>.</exception>
    /// <exception cref="RepositoryDataLoadException">Thrown when the entries could not be successfully loaded.</exception>
    Task<IReadOnlyList<RecipeRepositoryEntityData>> LoadRecipeEntriesByCategoryAsync(RecipeCategory category);

    /// <summary>
    /// Gets all the recipes filtered by a tag.
    /// </summary>
    /// <param name="tag">The tag to filter the recipes with.</param>
    /// <returns>A collection of filtered recipe entries.</returns>
    /// <exception cref="RepositoryDataLoadException">Thrown when the entries could not be successfully loaded.</exception>
    Task<IReadOnlyList<RecipeRepositoryEntityData>> LoadRecipeEntriesByTagAsync(string tag);

    /// <summary>
    /// Gets all the recipes filtered by a tag entity id.
    /// </summary>
    /// <param name="tagEntityId">The id of the tag entity to filter the recipes with.</param>
    /// <returns>A collection of filtered recipe entries.</returns>
    /// <exception cref="RepositoryDataLoadException">Thrown when the entries could not be successfully loaded.</exception>
    Task<IReadOnlyList<RecipeRepositoryEntityData>> LoadRecipeEntriesByTagEntityIdAsync(long tagEntityId);

    /// <summary>
    /// Gets all the recipes filtered by an author id.
    /// </summary>
    /// <param name="authorId">The author id to filter the recipes with.</param>
    /// <returns>A collection of filtered recipe entries.</returns>
    /// <exception cref="RepositoryDataLoadException">Thrown when the entries could not be successfully loaded.</exception>
    Task<IReadOnlyList<RecipeRepositoryEntityData>> LoadRecipeEntriesByAuthorIdAsync(ulong authorId);
}