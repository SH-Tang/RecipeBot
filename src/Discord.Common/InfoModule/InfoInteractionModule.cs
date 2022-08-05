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
using Discord.Commands;
using Discord.Interactions;
using WeekendBot.Utils;

namespace Discord.Common.InfoModule;

/// <summary>
/// Definition of slash commands that provide information about the bot.
/// </summary>
public class InfoInteractionModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly CommandService commandService;
    private readonly InteractionService interactionService;
    private readonly DiscordCommandInfoFactory commandInfoFactory;
    private readonly BotInformationService botInformationService;

    /// <summary>
    /// Creates a new instance of <see cref="InfoInteractionModule"/>.
    /// </summary>
    /// <param name="commandService">The <see cref="CommandService"/>.</param>
    /// <param name="interactionService">The <see cref="InteractionService"/>.</param>
    /// <param name="commandInfoFactory">The <see cref="DiscordCommandInfoFactory"/>.</param>
    /// <param name="botInformationService">The <see cref="BotInformationService"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    public InfoInteractionModule(CommandService commandService, InteractionService interactionService,
                                 DiscordCommandInfoFactory commandInfoFactory, BotInformationService botInformationService)
    {
        commandService.IsNotNull(nameof(commandService));
        interactionService.IsNotNull(nameof(interactionService));
        commandInfoFactory.IsNotNull(nameof(commandInfoFactory));
        botInformationService.IsNotNull(nameof(botInformationService));

        this.commandService = commandService;
        this.interactionService = interactionService;
        this.commandInfoFactory = commandInfoFactory;
        this.botInformationService = botInformationService;
    }

    [SlashCommand("help", "Provides information about all the available commands.")]
    public async Task GetHelpResponseAsync()
    {
        IEnumerable<DiscordCommandInfo> commandInfos = commandInfoFactory.Create(commandService.Commands, interactionService.SlashCommands);

        Embed embedSummaryInformation = await botInformationService.GetCommandInfoSummaries(commandInfos);
        await RespondAsync(null, new[]
        {
            embedSummaryInformation
        });
    }
}