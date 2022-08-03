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
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using WeekendBot.Utils;

namespace Discord.Common.Handlers;

/// <summary>
/// The command handler that uses the Discord interaction framework.
/// </summary>
public class InteractionDiscordCommandHandler
{
    private readonly DiscordSocketClient client;
    private readonly ILoggingService logger;
    private readonly DiscordCommandOptions commandOptions;
    private readonly InteractionService interactionService;
    private readonly IServiceProvider services;

    private bool isInitialized;

    /// <summary>
    /// Creates a new instance of <see cref="TextDiscordCommandHandler"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceProvider"/> for providing services.</param>
    /// <param name="interactionService">The <see cref="InteractionService"/>.</param>
    /// <param name="client">The <see cref="DiscordSocketClient"/>.</param>
    /// <param name="options">The <see cref="DiscordCommandOptions"/> to configure the handler with.</param>
    /// <param name="logger">The <see cref="ILoggingService"/> to use for the logging.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    public InteractionDiscordCommandHandler(
        IServiceProvider services, InteractionService interactionService, DiscordSocketClient client,
        IOptions<DiscordCommandOptions> options, ILoggingService logger)
    {
        services.IsNotNull(nameof(services));
        interactionService.IsNotNull(nameof(interactionService));
        client.IsNotNull(nameof(client));
        options.IsNotNull(nameof(options));
        logger.IsNotNull(nameof(logger));

        this.services = services;
        commandOptions = options.Value;
        this.logger = logger;

        this.interactionService = interactionService;
        interactionService.Log += OnLogEventHandler;

        this.client = client;
        client.Ready += ReadyEventHandler;
        client.InteractionCreated += InteractionCreatedEventHandler;

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
        return modulesTypesToAdd.Select(moduleType => interactionService.AddModuleAsync(moduleType, services))
                                .ToArray();
    }

    private async Task ReadyEventHandler()
    {
        // Context & Slash commands can be automatically registered, but this process needs to happen after the client enters the READY state.
        // Since Global Commands take around 1 hour to register, we should use a test guild to instantly update and test our commands.
#if DEBUG
        await interactionService.RegisterCommandsToGuildAsync(commandOptions.TestGuildId);
#else
        await _handler.RegisterCommandsGloballyAsync(true);
#endif
    }

    private Task OnLogEventHandler(LogMessage arg)
    {
        return logger.LogDebugAsync(arg.Message);
    }

    private async Task InteractionCreatedEventHandler(SocketInteraction interaction)
    {
        try
        {
            // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
            var context = new SocketInteractionContext(client, interaction);

            // Execute the incoming command.
            IResult? result = await interactionService.ExecuteCommandAsync(context, services);
            if (!result.IsSuccess)
            {
                var errorMessage = $"Command failed: {result.ErrorReason}";
                await context.Channel.SendMessageAsync(errorMessage);
                await logger.LogErrorAsync(errorMessage);
            }
        }
        catch
        {
            // If Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if (interaction.Type is InteractionType.ApplicationCommand)
            {
                await interaction.GetOriginalResponseAsync().ContinueWith(async msg => await msg.Result.DeleteAsync());
            }
        }
    }
}