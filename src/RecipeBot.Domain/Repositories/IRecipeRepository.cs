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
using System.Threading.Tasks;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Models;
using RecipeBot.Domain.Repositories.Data;

namespace RecipeBot.Domain.Repositories;

/// <summary>
/// Interface for describing a repository for handling with data persistence of recipes.
/// </summary>
public interface IRecipeRepository
{
    /// <summary>
    /// Saves a <see cref="RecipeModel"/>.
    /// </summary>
    /// <param name="model">The <see cref="RecipeModel"/> to save.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
    /// <exception cref="RepositoryDataSaveException">Thrown when the data could not be successfully saved.</exception>
    public Task SaveRecipeAsync(RecipeModel model);

    /// <summary>
    /// Deletes a recipe based on its id.
    /// </summary>
    /// <param name="id">The id of the recipe to delete.</param>
    /// <returns>A <see cref="RecipeEntryData"/> containing the information of the deleted recipe.</returns>
    /// <exception cref="RepositoryDataDeleteException">Thrown when the data could not be successfully deleted.</exception>
    Task<RecipeEntryData> DeleteRecipeAsync(long id);

    /// <summary>
    /// Gets a <see cref="RecipeData"/> based on its id.
    /// </summary>
    /// <param name="id">The id of the recipe to retrieve.</param>
    /// <returns>A <see cref="RecipeData"/>.</returns>
    /// <exception cref="RepositoryDataLoadException">Thrown when the data could not be successfully loaded.</exception>
    Task<RecipeData> GetRecipeAsync(long id);
}