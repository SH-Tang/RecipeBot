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
using RecipeBot.Domain.Models;

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
    public Task SaveRecipeAsync(RecipeModel model);
}