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
using System.Threading.Tasks;
using Discord;
using RecipeBot.Discord.Data;
using RecipeBot.Discord.Views;

namespace RecipeBot.Discord.Controllers;

/// <summary>
/// Interface for describing a controller dealing with recipe interactions.
/// </summary>
public interface IRecipeController
{
    /// <summary>
    /// Saves a recipe based its input arguments.
    /// </summary>
    /// <param name="modal">The <see cref="RecipeModal"/> to get the data from.</param>
    /// <param name="user">The <see cref="IUser"/> invoking the command.</param>
    /// <param name="category">The <see cref="DiscordRecipeCategory"/> the recipe belongs to.</param>
    /// <returns>The result of the save action.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="modal"/> and <paramref name="user"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/> is an invalid value of <see cref="DiscordRecipeCategory"/>.</exception>
    Task<ControllerResult<Embed>> SaveRecipeAsync(RecipeModal modal, IUser user, DiscordRecipeCategory category);

    /// <summary>
    /// Deletes a recipe based on its input arguments.
    /// </summary>
    /// <param name="idToDelete">The id of the recipe to delete.</param>
    /// <returns>The result of the delete action.</returns>
    Task<ControllerResult<string>> DeleteRecipeAsync(long idToDelete);

    /// <summary>
    /// Deletes a recipe based on its input arguments.
    /// </summary>
    /// <param name="idToDelete">The id of the recipe to delete.</param>
    /// <param name="user">The user the recipe belongs to.</param>
    /// <returns>The result of the delete action.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="user"/> is <c>null</c>.</exception>
    Task<ControllerResult<string>> DeleteRecipeAsync(long idToDelete, IUser user);

    /// <summary>
    /// Gets a recipe based on its input arguments.
    /// </summary>
    /// <param name="idToRetrieve">The id of the recipe to retrieve.</param>
    /// <returns>The result of the get action.</returns>
    Task<ControllerResult<Embed>> GetRecipeAsync(long idToRetrieve);
}