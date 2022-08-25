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
using WeekendBot.Utils;

namespace Discord.Common.InfoModule;

/// <summary>
/// Class containing information about Discord commands.
/// </summary>
public class DiscordCommandInfo
{
    /// <summary>
    /// Creates a new instance of <see cref="DiscordCommandInfo"/>.
    /// </summary>
    /// <param name="name">The name of the command.</param>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="name"/> is <c>null</c> or consists
    /// of whitespaces only.</exception>
    public DiscordCommandInfo(string name)
    {
        name.IsNotNullOrWhiteSpaces(nameof(name));
        Name = name;
    }

    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets or sets the summary of the command.
    /// </summary>
    public string? Summary { get; init; }
}