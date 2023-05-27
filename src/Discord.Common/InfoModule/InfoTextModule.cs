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
using System.Threading.Tasks;
using Common.Utils;
using Discord.Commands;
using Discord.Common.InfoModule.Data;
using Discord.Common.InfoModule.Services;
using Discord.Interactions;

namespace Discord.Common.InfoModule;

/// <summary>
/// Definition of text commands that provide information about the bot.
/// </summary>
public class InfoTextModule : ModuleBase<SocketCommandContext>
{
    private readonly BotInformationService botInformationService;
    private readonly DiscordCommandInfoFactory commandInfoFactory;
    private readonly CommandService commandService;
    private readonly InteractionService interactionService;

    /// <summary>
    /// Creates a new instance of <see cref="InfoTextModule"/>.
    /// </summary>
    /// <param name="commandService">The <see cref="CommandService"/>.</param>
    /// <param name="interactionService">The <see cref="InteractionService"/>.</param>
    /// <param name="commandInfoFactory">The <see cref="DiscordCommandInfoFactory"/>.</param>
    /// <param name="botInformationService">The <see cref="BotInformationService"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    public InfoTextModule(CommandService commandService, InteractionService interactionService,
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

    [Command("help")]
    [Commands.Summary("Provides information about all the available commands.")]
    public async Task GetHelpResponseAsync()
    {
        IEnumerable<DiscordCommandInfo> commandInfos = commandInfoFactory.Create(commandService.Commands, interactionService.SlashCommands);

        Embed embedSummaryInformation = botInformationService.GetCommandInfoSummaries(commandInfos);
        await ReplyAsync(null, false, embedSummaryInformation);
    }
}