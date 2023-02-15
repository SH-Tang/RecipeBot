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
using System.ComponentModel;
using System.Threading.Tasks;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Repositories.Data;

namespace RecipeBot.Domain.Repositories;

/// <summary>
/// Interface for describing a repository for handling data persistence of collections of recipe entries.
/// </summary>
public interface IRecipeDataEntryCollectionRepository
{
    /// <summary>
    /// Gets all the recipes.
    /// </summary>
    /// <returns>A collection of recipe entries.</returns>
    Task<IReadOnlyList<RecipeEntryData>> LoadRecipeEntriesAsync();

    /// <summary>
    /// Gets all the recipes belonging to a <see cref="RecipeCategory"/>.
    /// </summary>
    /// <param name="category">The <see cref="RecipeCategory"/> the recipes belong to.</param>
    /// <returns>A collection of recipe entries belonging to the <see cref="RecipeCategory"/>.</returns>
    /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/> is an invalid <see cref="RecipeCategory"/>.</exception>
    Task<IReadOnlyList<RecipeEntryData>> LoadRecipeEntriesByCategoryAsync(RecipeCategory category);
}