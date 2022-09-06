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
using System.Linq;
using System.Threading.Tasks;
using Common.Utils;
using Discord.Common.Options;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace Discord.Common.Handlers;

/// <summary>
/// Base implementation for Discord command handlers.
/// </summary>
public abstract class DiscordCommandHandlerBase
{
    protected readonly DiscordSocketClient Client;
    protected readonly DiscordCommandOptions CommandOptions;
    protected readonly ILoggingService Logger;
    protected readonly IServiceProvider Services;

    private bool isInitialized;

    /// <summary>
    /// Creates a new instance of <see cref="DiscordCommandHandlerBase"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceProvider"/> for providing services.</param>
    /// <param name="client">The <see cref="DiscordSocketClient"/>.</param>
    /// <param name="options">The <see cref="DiscordCommandOptions"/> to configure the handler with.</param>
    /// <param name="logger">The <see cref="ILoggingService"/> to use for the logging.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    protected DiscordCommandHandlerBase(
        IServiceProvider services, DiscordSocketClient client, IOptions<DiscordCommandOptions> options, ILoggingService logger)
    {
        services.IsNotNull(nameof(services));
        client.IsNotNull(nameof(client));
        options.IsNotNull(nameof(options));
        logger.IsNotNull(nameof(logger));

        Services = services;
        CommandOptions = options.Value;
        Logger = logger;

        Client = client;

        isInitialized = false;
    }

    /// <summary>
    /// The function that describes how a module type should be added to the handler.
    /// </summary>
    /// <returns>A <see cref="Func{TResult}"/> that describes how module types are added.</returns>
    protected abstract Func<Type, IServiceProvider, Task> AddModuleFunc { get; }

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

            PostProcessInitialization();
            isInitialized = true;
        }
        else
        {
            throw new InvalidOperationException("Handler is already initialized.");
        }
    }

    protected async Task LogEventHandler(LogMessage arg)
    {
        string message = arg.Message;
        if (!string.IsNullOrWhiteSpace(message))
        {
            await Logger.LogDebugAsync($"{arg.Source} - {message}");
        }

        Exception exception = arg.Exception;
        if (exception != null)
        {
            await Logger.LogErrorAsync($"{arg.Source} - {exception.Message}");

            string? stackTrace = exception.StackTrace;
            if (!string.IsNullOrWhiteSpace(stackTrace))
            {
                await Logger.LogErrorAsync(stackTrace);
            }
        }
    }

    protected virtual void PostProcessInitialization() {}

    private IEnumerable<Task> CreateAddingModuleTasks(IEnumerable<Type> modulesTypesToAdd)
    {
        return modulesTypesToAdd.Select(moduleType => AddModuleFunc(moduleType, Services))
                                .ToArray();
    }
}