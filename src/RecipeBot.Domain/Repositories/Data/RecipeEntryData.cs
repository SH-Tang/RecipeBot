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

namespace RecipeBot.Domain.Repositories.Data;

/// <summary>
/// Class representing a simplified recipe data entry.
/// </summary>
public class RecipeEntryData
{
    /// <summary>
    /// Creates a new instance of <see cref="RecipeEntryData"/>.
    /// </summary>
    /// <param name="id">The id of the recipe.</param>
    /// <param name="title">The title of the recipe.</param>
    /// <param name="authorId">The id of the author of the recipe.</param>
    public RecipeEntryData(long id, string title, ulong authorId)
    {
        Id = id;
        Title = title;
        AuthorId = authorId;
    }

    /// <summary>
    /// Gets the id of the recipe.
    /// </summary>
    public long Id { get; }

    /// <summary>
    /// Gets the title of the recipe.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the id of the author of the recipe.
    /// </summary>
    public ulong AuthorId { get; }
}