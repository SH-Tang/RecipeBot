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
using Discord.Commands;
using Discord.Common.InfoModule.Data;
using Discord.Common.InfoModule.Services;
using Discord.Common.Options;
using Discord.Interactions;
using Microsoft.Extensions.Options;

namespace Discord.Common.InfoModule.Controllers;

/// <summary>
/// Controller to deal with interactions related to providing information about a Discord bot.
/// </summary>
public class DiscordBotInformationController : IDiscordBotInformationController
{
    private readonly BotInformationService botInformationService;
    private readonly DiscordCommandInfoFactory commandInfoFactory;
    private readonly CommandService commandService;
    private readonly InteractionService interactionService;

    /// <summary>
    /// Creates a new instance of <see cref="DiscordBotInformationController"/>.
    /// </summary>
    /// <param name="commandService">The <see cref="CommandService"/>.</param>
    /// <param name="interactionService">The <see cref="InteractionService"/>.</param>
    /// <param name="commandOptions">The <see cref="DiscordCommandOptions"/> that were used to configure the application with.</param>
    /// <param name="botInformation">The <see cref="BotInformation"/> to supply additional information about the bot.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    public DiscordBotInformationController(CommandService commandService, InteractionService interactionService,
                                           IOptions<DiscordCommandOptions> commandOptions, IOptions<BotInformation> botInformation)
    {
        commandService.IsNotNull(nameof(commandService));
        interactionService.IsNotNull(nameof(interactionService));
        commandOptions.IsNotNull(nameof(commandOptions));
        botInformation.IsNotNull(nameof(botInformation));

        this.commandService = commandService;
        this.interactionService = interactionService;
        this.commandInfoFactory = new DiscordCommandInfoFactory();
        this.botInformationService = new BotInformationService(botInformation);
    }

    /// <summary>
    /// Gets the information about the available bot command.
    /// </summary>
    /// <returns>A result containing all the available commands for the bot.</returns>
    public Embed GetAvailableBotCommands()
    {
        IEnumerable<DiscordCommandInfo> commandInfos = DiscordCommandInfoFactory.Create(interactionService.SlashCommands);

        return botInformationService.GetCommandInfoSummaries(commandInfos);
    }
}