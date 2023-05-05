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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Common.Utils;
using Discord;
using Discord.Commands;
using Discord.Common;
using Discord.Common.Handlers;
using Discord.Common.InfoModule;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeBot.Discord;
using RecipeBot.Persistence;

namespace RecipeBot;

/// <summary>
/// The RecipeBot application.
/// </summary>
public class RecipeBotApplication
{
    private readonly IConfiguration configurationRoot;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeBotApplication"/>.
    /// </summary>
    /// <param name="configurationFilePath">The file path to the configuration settings.</param>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="configurationFilePath"/> is not a valid json
    /// file path.</exception>
    public RecipeBotApplication(string configurationFilePath)
    {
        configurationFilePath.IsExistingFilePath(nameof(configurationFilePath));
        configurationFilePath.IsValidArgument(arg => Path.GetExtension(arg).ToLower() == ".json",
                                              $"{nameof(configurationFilePath)} is not a valid json file.",
                                              nameof(configurationFilePath));

        IConfigurationBuilder builder = new ConfigurationBuilder()
                                        .SetBasePath(AppContext.BaseDirectory)
                                        .AddJsonFile(configurationFilePath);

        configurationRoot = builder.Build();
    }

    /// <summary>
    /// Runs the application.
    /// </summary>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Thrown when something went wrong while running.</exception>
    public async Task Run()
    {
        var services = new RecipeBotApplicationServiceProvider(configurationRoot);
        using(ServiceProvider serviceProvider = services.GetServiceProvider())
        {
            await SetupDatabase(serviceProvider);
            await ConfigureDiscordClient(serviceProvider);
            await ConfigureCommandService(serviceProvider);
            await ConfigureCommandHandlingService(serviceProvider);

            await Task.Delay(Timeout.Infinite);
        }
    }

    private async Task ConfigureDiscordClient(IServiceProvider services)
    {
        var discordClient = services.GetRequiredService<DiscordSocketClient>();
        discordClient.Log += message => LogAsync(services, message);

        string? token = configurationRoot["Token"];
        await discordClient.LoginAsync(TokenType.Bot, token);
        await discordClient.StartAsync();
    }

    private static async Task ConfigureCommandHandlingService(IServiceProvider services)
    {
        var textCommandHandlingService = services.GetRequiredService<TextDiscordCommandHandler>();
        await textCommandHandlingService.InitializeHandlerAsync(new[]
        {
            typeof(InfoTextModule)
        });

        var interactionCommandHandlingServices = services.GetRequiredService<InteractionDiscordCommandHandler>();
        await interactionCommandHandlingServices.InitializeHandlerAsync(new[]
        {
            typeof(InfoInteractionModule),
            typeof(RecipeInteractionModule),
            typeof(RecipeEntriesInteractionModule),
            typeof(RecipeTagEntriesInteractionModule)
        });
    }

    private static Task ConfigureCommandService(IServiceProvider services)
    {
        var commandService = services.GetRequiredService<CommandService>();
        commandService.Log += message => LogAsync(services, message);

        return Task.CompletedTask;
    }

    private static Task LogAsync(IServiceProvider services, LogMessage msg)
    {
        var logger = services.GetRequiredService<ILoggingService>();
        logger.LogInfoAsync(msg.Message);

        return Task.CompletedTask;
    }

    private static async Task SetupDatabase(IServiceProvider services)
    {
        using(IServiceScope scope = services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<RecipeBotDbContext>();
            await context.Database.EnsureCreatedAsync();
        }
    }
}