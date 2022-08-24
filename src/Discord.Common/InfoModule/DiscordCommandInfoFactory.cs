// Copyright (C) 2022 Dennis Tang. All rights reserved.
//
// This file is part of WeekendBot.
//
// WeekendBot is free software: you can redistribute it and/or modify
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
using System.Linq;
using Discord.Commands;
using Discord.Common.Options;
using Discord.Interactions;
using Microsoft.Extensions.Options;
using WeekendBot.Utils;

namespace Discord.Common.InfoModule;

/// <summary>
/// Factory to create instances eof <see cref="DiscordCommandInfo"/>.
/// </summary>
public class DiscordCommandInfoFactory
{
    private readonly DiscordCommandOptions commandOptions;

    /// <summary>
    /// Creates a new instance of <see cref="DiscordCommandInfoFactory"/>.
    /// </summary>
    /// <param name="options">The <see cref="DiscordCommandOptions"/> that were used to configure the application with.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <c>null</c>.</exception>
    public DiscordCommandInfoFactory(IOptions<DiscordCommandOptions> options)
    {
        options.IsNotNull(nameof(options));
        commandOptions = options.Value;
    }

    public IEnumerable<DiscordCommandInfo> Create(IEnumerable<CommandInfo> textCommands, IEnumerable<SlashCommandInfo> slashCommands)
    {
        textCommands.IsNotNull(nameof(textCommands));
        slashCommands.IsNotNull(nameof(slashCommands));

        var discordCommandInfos = new List<DiscordCommandInfo>();
        discordCommandInfos.AddRange(textCommands.Select(c => new DiscordCommandInfo(FormatTextCommand(c.Name))
        {
            Summary = c.Summary
        }));

        discordCommandInfos.AddRange(slashCommands.Select(c => new DiscordCommandInfo(FormatSlashCommand(c.Name))
        {
            Summary = c.Description
        }));

        return discordCommandInfos;
    }

    private string FormatTextCommand(string slashCommandName)
    {
        return $"{commandOptions.CommandPrefix}{slashCommandName}";
    }

    private static string FormatSlashCommand(string slashCommandName)
    {
        return $"/{slashCommandName}";
    }
}