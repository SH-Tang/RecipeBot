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
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using WeekendBot.Utils;

namespace Discord.Common.Handlers
{
    /// <summary>
    /// The text handler to deal with Discord commands.
    /// </summary>
    public class TextDiscordCommandHandler
    {
        private readonly DiscordSocketClient client;
        private readonly ILoggingService logger;
        private readonly DiscordCommandOptions commandOptions;
        private readonly CommandService commandService;
        private readonly IServiceProvider services;

        private bool isInitialized;

        /// <summary>
        /// Creates a new instance of <see cref="TextDiscordCommandHandler"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceProvider"/> for providing services.</param>
        /// <param name="commandService">The <see cref="CommandService"/>.</param>
        /// <param name="client">The <see cref="DiscordSocketClient"/>.</param>
        /// <param name="options">The <see cref="DiscordCommandOptions"/> to configure the handler with.</param>
        /// <param name="logger">The <see cref="ILoggingService"/> to use for the logging.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public TextDiscordCommandHandler(
            IServiceProvider services, CommandService commandService, DiscordSocketClient client,
            IOptions<DiscordCommandOptions> options, ILoggingService logger)
        {
            services.IsNotNull(nameof(services));
            commandService.IsNotNull(nameof(commandService));
            client.IsNotNull(nameof(client));
            options.IsNotNull(nameof(options));
            logger.IsNotNull(nameof(logger));

            this.services = services;
            this.logger = logger;
            
            this.commandService = commandService;
            commandService.CommandExecuted += CommandExecutedEventHandler;
            commandService.Log += LogEventHandler;

            this.client = client;
            this.client.MessageReceived += MessageReceivedEventHandler;

            commandOptions = options.Value;

            isInitialized = false;
        }


        /// <summary>
        /// Initializes the handler with a collection of modules.
        /// </summary>
        /// <param name="moduleTypes">The collection of <see cref="Type"/> of modules to add.</param>
        /// <returns>A <see cref="Task"/> indicating the status of the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="moduleTypes"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when a duplicate module <see cref="Type"/> was added.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the handler is already initialized
        /// or when an invalid <see cref="Type"/> was added.</exception>
        public async Task InitializeHandlerAsync(IEnumerable<Type> moduleTypes)
        {
            moduleTypes.IsNotNull(nameof(moduleTypes));

            if (!isInitialized)
            {
                IEnumerable<Task> addingModuleTypeTasks = CreateAddingModuleTasks(moduleTypes);
                await Task.WhenAll(addingModuleTypeTasks);
                isInitialized = true;
            }
            else
            {
                throw new InvalidOperationException("Handler is already initialized.");
            }
        }

        private IEnumerable<Task> CreateAddingModuleTasks(IEnumerable<Type> modulesTypesToAdd)
        {
            return modulesTypesToAdd.Select(moduleType => commandService.AddModuleAsync(moduleType, services))
                                    .ToArray();
        }

        private async Task MessageReceivedEventHandler(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null)
            {
                return;
            }

            // Create a number to track where the prefix ends and the command begins
            var argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix(commandOptions.CommandPrefix, ref argPos) 
                  || message.HasMentionPrefix(client.CurrentUser, ref argPos)) 
                || message.Author.IsBot)
            {
                return;
            }

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await commandService.ExecuteAsync(context, argPos, services);
        }

        private async Task CommandExecutedEventHandler(
            Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result)
        {
            if (!commandInfo.IsSpecified || result.IsSuccess)
            {
                return;
            }

            var errorMessage = $"Command {commandInfo.Value.Name} failed: {result.ErrorReason}";
            await commandContext.Channel.SendMessageAsync(errorMessage);
            await logger.LogErrorAsync(errorMessage);
        }

        private Task LogEventHandler(LogMessage arg)
        {
            return logger.LogDebugAsync(arg.Message);
        }
    }
}