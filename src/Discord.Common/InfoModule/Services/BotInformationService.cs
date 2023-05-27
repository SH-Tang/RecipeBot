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
using System.Collections.Generic;
using Common.Utils;
using Discord.Common.InfoModule.Data;
using Microsoft.Extensions.Options;

namespace Discord.Common.InfoModule.Services;

/// <summary>
/// Service for providing information about the bot.
/// </summary>
public class BotInformationService
{
    private readonly BotInformation botInformation;

    /// <summary>
    /// Creates a new instance of <see cref="BotInformationService"/>.
    /// </summary>
    /// <param name="botInfo">The <see cref="BotInformation"/> to supply additional information about the bot.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="botInfo"/> is <c>null</c>.</exception>
    public BotInformationService(IOptions<BotInformation> botInfo)
    {
        botInfo.IsNotNull(nameof(botInfo));
        botInformation = botInfo.Value;
    }

    /// <summary>
    /// Gets an <see cref="Embed"/> that contains the summaries of all available commands.
    /// </summary>
    /// <returns>A <see cref="Embed"/> containing summaries of all available commands.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="commandInfos"/> is <c>null</c>.</exception>
    public Embed GetCommandInfoSummaries(IEnumerable<DiscordCommandInfo> commandInfos)
    {
        commandInfos.IsNotNull(nameof(commandInfos));

        EmbedBuilder embedBuilder = CreateEmbedBuilder();
        foreach (DiscordCommandInfo command in commandInfos)
        {
            string embedFieldText = command.Summary ?? $"No description available.{Environment.NewLine}";
            embedBuilder.AddField(command.Name, embedFieldText);
        }

        return embedBuilder.Build();
    }

    private EmbedBuilder CreateEmbedBuilder()
    {
        AuthorInformation? authorInformation = botInformation.AuthorInformation;
        var embedBuilder = new EmbedBuilder
        {
            Title = GetSummaryTitle(),
            Color = Color.Blue,
            Url = botInformation.BotInformationUrl,
            Author = new EmbedAuthorBuilder
            {
                Name = authorInformation?.AuthorName,
                Url = authorInformation?.AuthorUrl,
                IconUrl = authorInformation?.AuthorAvatarUrl
            }
        };
        return embedBuilder;
    }

    private string GetSummaryTitle()
    {
        return botInformation.BotName == null
            ? "Available commands"
            : $"Available commands for {botInformation.BotName}";
    }
}