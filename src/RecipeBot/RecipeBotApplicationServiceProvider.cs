﻿// Copyright (C) 2022 Dennis Tang. All rights reserved.
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
using Common.Utils;
using Discord.Commands;
using Discord.Common.Handlers;
using Discord.Common.InfoModule;
using Discord.Common.InfoModule.Controllers;
using Discord.Common.InfoModule.Data;
using Discord.Common.Options;
using Discord.Common.Providers;
using Discord.Common.Services;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeBot.Controllers;
using RecipeBot.Discord.Controllers;
using RecipeBot.Domain.Factories;
using RecipeBot.Domain.Repositories;
using RecipeBot.Persistence;
using RecipeBot.Providers;
using RecipeBot.Services;

namespace RecipeBot;

/// <summary>
/// Provides all the necessary services for the <see cref="RecipeBotApplication"/>.
/// </summary>
public class RecipeBotApplicationServiceProvider
{
    private readonly IConfiguration configuration;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeBotApplicationServiceProvider"/>.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/> to initialize the services with.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration"/> is <c>null</c>.</exception>
    public RecipeBotApplicationServiceProvider(IConfiguration configuration)
    {
        configuration.IsNotNull(nameof(configuration));
        this.configuration = configuration;
    }

    /// <summary>
    /// Gets the configured <see cref="ServiceProvider"/> for the application.
    /// </summary>
    /// <returns>The configured <see cref="ServiceProvider"/>.</returns>
    public ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        ConfigureOptions(services);

        return services.BuildServiceProvider(true);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        var socketConfig = new DiscordSocketConfig
        {
            UseInteractionSnowflakeDate = false
        };

        services.AddSingleton(socketConfig)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<InteractionDiscordCommandHandler>()
                .AddSingleton<ILoggingService, RecipeBotLoggingService>()
                .AddSingleton<DiscordCharacterLimitProvider>()
                .AddSingleton<IRecipeModelCharacterLimitProvider>(x => x.GetRequiredService<DiscordCharacterLimitProvider>())
                .AddSingleton<IMessageCharacterLimitProvider>(x => x.GetRequiredService<DiscordCharacterLimitProvider>())
                .AddTransient<IUserDataProvider, UserDataProvider>()
                .AddTransient<IDiscordBotInformationController, DiscordBotInformationController>()
                .AddScoped<IRecipeController, RecipeController>()
                .AddScoped<IRecipeEntriesController, RecipeEntriesController>()
                .AddScoped<IRecipeTagEntriesController, RecipeTagEntriesController>()
                .AddScoped<IAuthorController, AuthorController>()
                .AddDbContext<RecipeBotDbContext>(options => options.UseSqlite(configuration.GetConnectionString("DefaultConnection")))
                .AddScoped<IRecipeRepository, RecipeRepository>()
                .AddScoped<IRecipeCollectionRepository, RecipeCollectionRepository>()
                .AddScoped<IRecipeTagRepository, RecipeTagRepository>()
                .AddScoped<IAuthorRepository, AuthorRepository>();
    }

    private void ConfigureOptions(IServiceCollection services)
    {
        services.ConfigureAndValidate<DiscordCommandOptions>(
                    options => configuration.GetSection(DiscordCommandOptions.SectionKey)
                                            .Bind(options))
                .ConfigureAndValidate<BotInformation>(
                    options => configuration.GetSection(BotInformation.SectionKey)
                                            .Bind(options));
    }
}