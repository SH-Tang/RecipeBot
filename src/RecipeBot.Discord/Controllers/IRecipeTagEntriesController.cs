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
using System.Threading.Tasks;

namespace RecipeBot.Discord.Controllers;

/// <summary>
/// Interface for describing a controller dealing with interactions with recipe tag entries.
/// </summary>
public interface IRecipeTagEntriesController
{
    /// <summary>
    /// Lists all tags.
    /// </summary>
    /// <returns>A collection of messages containing formatted recipe tag entries.</returns>
    Task<ControllerResult<IReadOnlyList<string>>> ListAllTagsAsync();

    /// <summary>
    /// Deletes a tag based on its input arguments.
    /// </summary>
    /// <param name="idToDelete">The id of the tag to delete.</param>
    /// <returns>The result of the delete action.</returns>
    Task<ControllerResult<string>> DeleteTagAsync(long idToDelete);
}