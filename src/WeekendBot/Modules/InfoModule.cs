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
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using WeekendBot.Utils;

namespace WeekendBot.Modules
{
    /// <summary>
    /// Definition of commands that provide information about the bot.
    /// </summary>
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService service;

        /// <summary>
        /// Creates a new instance of <see cref="InfoModule"/>.
        /// </summary>
        /// <param name="service">The <see cref="CommandService"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="service"/> is <c>null</c>.</exception>
        public InfoModule(CommandService service)
        {
            service.IsNotNull(nameof(service));
            this.service = service;
        }

        [Command("help")]
        [Summary("Provides information about all the available commands.")]
        public Task GetHelpResponseAsync()
        {
            var embedBuilder = new EmbedBuilder();
            foreach (CommandInfo command in service.Commands)
            {
                // Get the command Summary attribute information
                string embedFieldText = command.Summary ?? $"No description available{Environment.NewLine}";

                embedBuilder.AddField(command.Name, embedFieldText);
            }

            return ReplyAsync("List of available commands", false, embedBuilder.Build());
        }
    }
}