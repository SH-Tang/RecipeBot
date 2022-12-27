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
using System.Threading.Tasks;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Models;

namespace RecipeBot.Domain.Repositories;

/// <summary>
/// Interface for defining a repository for recipes.
/// </summary>
public interface IRecipeRepository
{
    /// <summary>
    /// Saves the recipe based on its input arguments.
    /// </summary>
    /// <param name="recipe">The <see cref="RecipeModel"/> to save.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="recipe"/> is <c>null</c>.</exception>
    Task SaveRecipeAsync(RecipeModel recipe);

    /// <summary>
    /// Deletes a recipe based on its input arguments.
    /// </summary>
    /// <param name="id">The id of the recipe to delete</param>
    /// <returns>The <see cref="RecipeData"/> which was deleted; <c>null</c> if deletion was unsuccessful.</returns>
    Task<RecipeData?> DeleteRecipeAsync(int id);

    /// <summary>
    /// Gets a <see cref="RecipeData"/> by the id.
    /// </summary>
    /// <param name="id">The id to retrieve a recipe with.</param>
    /// <returns>A <see cref="RecipeData"/>; <c>null</c> if not found.</returns>
    Task<RecipeData?> GetRecipeByIdAsync(int id);

    /// <summary>
    /// Gets all stored recipes.
    /// </summary>
    /// <returns>A collection of <see cref="RecipeData"/>.</returns>
    Task<IEnumerable<RecipeData>> GetAllRecipes();
}