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
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Common.Options;
using Discord.Interactions;
using Microsoft.Extensions.Options;
using WeekendBot.Utils;

namespace Discord.Common.InfoModule
{
    /// <summary>
    /// Definition of text commands that provide information about the bot.
    /// </summary>
    public class InfoTextModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService commandService;
        private readonly InteractionService interactionService;
        private readonly DiscordCommandOptions commandOptions;
        private readonly BotInformationService botInformationService;

        /// <summary>
        /// Creates a new instance of <see cref="InfoTextModule"/>.
        /// </summary>
        /// <param name="commandService">The <see cref="CommandService"/>.</param>
        /// <param name="interactionService">The <see cref="InteractionService"/>.</param>
        /// <param name="commandOptions">The <see cref="DiscordCommandOptions"/> that were used to configure
        /// the application with.</param>
        /// <param name="botInformationService">The <see cref="BotInformationService"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public InfoTextModule(CommandService commandService, InteractionService interactionService,
                              IOptions<DiscordCommandOptions> commandOptions, BotInformationService botInformationService)
        {
            commandService.IsNotNull(nameof(commandService));
            interactionService.IsNotNull(nameof(interactionService));
            commandOptions.IsNotNull(nameof(commandOptions));
            botInformationService.IsNotNull(nameof(botInformationService));

            this.commandService = commandService;
            this.interactionService = interactionService;
            this.commandOptions = commandOptions.Value;
            this.botInformationService = botInformationService;
        }

        [Command("help")]
        [Commands.Summary("Provides information about all the available commands.")]
        public async Task GetHelpResponseAsync()
        {
            var discordCommandInfos = new List<DiscordCommandInformation>();
            discordCommandInfos.AddRange(commandService.Commands.Select(c => new DiscordCommandInformation(FormatTextCommand(c.Name))
            {
                Summary = c.Summary
            }));

            discordCommandInfos.AddRange(interactionService.SlashCommands.Select(c => new DiscordCommandInformation(FormatSlashCommand(c.Name))
            {
                Summary = c.Description
            }));

            Embed embedSummaryInformation = await botInformationService.GetCommandInfoSummaries(discordCommandInfos);
            await ReplyAsync(null, false, embedSummaryInformation);
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
}