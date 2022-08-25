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

using System.ComponentModel.DataAnnotations;

namespace Discord.Common.InfoModule;

/// <summary>
/// Class containing information about the author.
/// </summary>
public class AuthorInformation
{
    /// <summary>
    /// Gets or sets the author of the bot.
    /// </summary>
    public string? AuthorName { get; set; }

    /// <summary>
    /// Gets or sets the url of the author of the bot.
    /// </summary>
    [Url]
    public string? AuthorAvatarUrl { get; set; }

    /// <summary>
    /// Gets or sets the url of the author
    /// </summary>
    [Url]
    public string? AuthorUrl { get; set; }
}