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

using System.Collections.Generic;
using System.Threading.Tasks;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Repositories.Data;

namespace RecipeBot.Domain.Repositories;

/// <summary>
/// Interface for describing a repository for handling data persistence of authors.
/// </summary>
public interface IAuthorRepository
{
    /// <summary>
    /// Deletes an author based on the author id.
    /// </summary>
    /// <param name="authorId">The id of the author to be deleted.</param>
    /// <returns>A <see cref="Task{TResult}"/>.</returns>
    /// <exception cref="RepositoryDataDeleteException">Thrown when the data could not be successfully deleted.</exception>
    Task DeleteAuthorAsync(ulong authorId);

    /// <summary>
    /// Deletes an author based on the author entity id.
    /// </summary>
    /// <param name="entityId">The id of the author entity to be deleted.</param>
    /// <returns>A <see cref="AuthorRepositoryEntityData"/> containing the information of the deleted author.</returns>
    /// <exception cref="RepositoryDataDeleteException">Thrown when the data could not be successfully deleted.</exception>
    Task<AuthorRepositoryEntityData> DeleteAuthorAsync(long entityId);

    /// <summary>
    /// Gets all the authors.
    /// </summary>
    /// <returns>A collection of author entities.</returns>
    /// <exception cref="RepositoryDataLoadException">Thrown when the authors could not be loaded successfully.</exception>
    Task<IReadOnlyCollection<AuthorRepositoryEntityData>> LoadAuthorsAsync();
}