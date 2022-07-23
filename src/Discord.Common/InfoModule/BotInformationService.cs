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
using System.Threading.Tasks;
using WeekendBot.Utils;

namespace Discord.Common.InfoModule;

/// <summary>
/// Service for providing information about the bot.
/// </summary>
public class BotInformationService : IBotInformationService
{
    public Task<Embed> GetCommandInfoSummaries(IEnumerable<DiscordCommandInformation> commandInfos)
    {
        commandInfos.IsNotNull(nameof(commandInfos));

        var embedBuilder = new EmbedBuilder();
        foreach (DiscordCommandInformation command in commandInfos)
        {
            string embedFieldText = command.Summary ?? $"No description available.{Environment.NewLine}";

            embedBuilder.AddField(command.Name, embedFieldText);
        }
        
        return Task.FromResult(embedBuilder.Build());
    }
}