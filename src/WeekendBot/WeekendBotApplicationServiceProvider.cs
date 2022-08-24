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
using Discord.Commands;
using Discord.Common;
using Discord.Common.Handlers;
using Discord.Common.InfoModule;
using Discord.Common.Options;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeekendBot.Components;
using WeekendBot.Core;
using WeekendBot.Discord;
using WeekendBot.Domain.Factories;
using WeekendBot.Services;
using WeekendBot.Utils;

namespace WeekendBot;

/// <summary>
/// Provides all the necessary services for the <see cref="WeekendBotApplication"/>.
/// </summary>
public class WeekendBotApplicationServiceProvider
{
    private readonly IConfiguration configuration;

    /// <summary>
    /// Creates a new instance of <see cref="WeekendBotApplicationServiceProvider"/>.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/> to initialize the services with.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration"/> is <c>null</c>.</exception>
    public WeekendBotApplicationServiceProvider(IConfiguration configuration)
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

    private static void ConfigureServices(IServiceCollection services)
    {
        var socketConfig = new DiscordSocketConfig
        {
            UseInteractionSnowflakeDate = false
        };

        services.AddSingleton(socketConfig)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<TextDiscordCommandHandler>()
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<InteractionDiscordCommandHandler>()
                .AddSingleton<ILoggingService, ConsoleLoggingService>()
                .AddSingleton<IRecipeDomainEntityCharacterLimitProvider, DiscordCharacterLimitProvider>()
                .AddTransient<ITimeProvider, TimeProvider>()
                .AddTransient<IWeekendInquiryService, WeekendInquiryService>()
                .AddTransient<DiscordCommandInfoFactory>()
                .AddTransient<BotInformationService>()
                .AddTransient<RecipeModalResponseService>();
    }

    private void ConfigureOptions(IServiceCollection services)
    {
        services.ConfigureAndValidate<DiscordCommandOptions>(
                    options => configuration.GetSection(DiscordCommandOptions.SectionKey)
                                            .Bind(options))
                .ConfigureAndValidate<StringFormatOptions>(
                    options => configuration.GetSection(StringFormatOptions.SectionKey)
                                            .Bind(options))
                .ConfigureAndValidate<BotInformation>(
                    options => configuration.GetSection(BotInformation.SectionKey)
                                            .Bind(options));
    }
}