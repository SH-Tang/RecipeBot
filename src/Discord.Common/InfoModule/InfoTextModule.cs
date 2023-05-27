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
using System.Threading.Tasks;
using Common.Utils;
using Discord.Commands;

namespace Discord.Common.InfoModule;

/// <summary>
/// Definition of text commands that provide information about the bot.
/// </summary>
public class InfoTextModule : ModuleBase<SocketCommandContext>
{
    private readonly IDiscordBotInformationController controller;
    private readonly ILoggingService logger;

    /// <summary>
    /// Creates a new instance of <see cref="InfoTextModule"/>.
    /// </summary>
    /// <param name="controller">The <see cref="IDiscordBotInformationController"/> to coordinate the interactions.</param>
    /// <param name="logger">The logger to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    public InfoTextModule(IDiscordBotInformationController controller, ILoggingService logger)
    {
        controller.IsNotNull(nameof(controller));
        logger.IsNotNull(nameof(logger));

        this.controller = controller;
        this.logger = logger;
    }

    [Command("help")]
    [Summary("Provides information about all the available commands.")]
    public async Task GetHelpResponseAsync()
    {
        try
        {
            Embed embedSummaryInformation = controller.GetAvailableBotCommands();
            await ReplyAsync(null, false, embedSummaryInformation);
        }
        catch (Exception e)
        {
            Task[] tasks =
            {
                ReplyAsync(e.Message),
                logger.LogErrorAsync(e)
            };

            await Task.WhenAll(tasks);
        }
    }
}