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
using System.Linq;
using Common.Utils;
using Discord.Common.InfoModule.Data;
using Discord.Interactions;

namespace Discord.Common.InfoModule.Services;

/// <summary>
/// Factory to create instances eof <see cref="DiscordCommandInfo"/>.
/// </summary>
public static class DiscordCommandInfoFactory
{
    /// <summary>
    /// Creates a collection of <see cref="DiscordCommandInfo"/> based on its input arguments.
    /// </summary>
    /// <param name="slashCommands">The collection of <see cref="SlashCommandInfo"/>.</param>
    /// <returns>A collection of <see cref="DiscordCommandInfo"/>.</returns>
    public static IEnumerable<DiscordCommandInfo> Create(IEnumerable<SlashCommandInfo> slashCommands)
    {
        slashCommands.IsNotNull(nameof(slashCommands));

        return slashCommands.Select(c => new DiscordCommandInfo(FormatSlashCommand(c.Name))
        {
            Summary = c.Description
        }).ToArray();
    }

    private static string FormatSlashCommand(string slashCommandName)
    {
        return $"/{slashCommandName}";
    }
}