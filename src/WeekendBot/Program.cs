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
using Discord.Common;
using Discord.Common.Handler;
using Discord.Common.InfoModule;
using Discord.Common.Options;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeekendBot.Components;
using WeekendBot.Core;
using WeekendBot.Modules;
using WeekendBot.Services;

namespace WeekendBot
{
    internal class Program
    {
        private readonly IConfiguration configurationRoot;
        private DiscordSocketClient discordClient = null!;

        private Program()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                                            .SetBasePath(AppContext.BaseDirectory)
                                            .AddJsonFile("config.json");

            configurationRoot = builder.Build();
        }

        public static Task Main(string[] args)
        {
            return new Program().MainAsync();
        }

        private async Task MainAsync()
        {
            using (ServiceProvider services = GetServiceProvider())
            {
                await ConfigureDiscordClient(services);
                await ConfigureCommandService(services);
                await ConfigureCommandHandlingService(services);

                // Block this task until the program is closed.
                await Task.Delay(-1);
            }
        }

        private async Task ConfigureDiscordClient(IServiceProvider services)
        {
            discordClient = services.GetRequiredService<DiscordSocketClient>();
            discordClient.Log += message => LogAsync(services, message);

            string token = configurationRoot["Token"];
            await discordClient.LoginAsync(TokenType.Bot, token);
            await discordClient.StartAsync();
        }

        private static async Task ConfigureCommandHandlingService(IServiceProvider services)
        {
            var commandHandlingService = services.GetRequiredService<ExplicitDiscordCommandHandler>();
            await commandHandlingService.InitializeHandlerAsync(new[]
            {
                typeof(WeekendModule),
                typeof(InfoModule)
            });
        }

        private static async Task ConfigureCommandService(IServiceProvider services)
        {
            var commandService = services.GetRequiredService<CommandService>();
            commandService.Log += message => LogAsync(services, message);

            await Task.CompletedTask;
        }

        private static Task LogAsync(IServiceProvider services, LogMessage msg)
        {
            var logger = services.GetRequiredService<ILoggingService>();
            logger.LogInfoAsync(msg.Message);

            return Task.CompletedTask;
        }

        private ServiceProvider GetServiceProvider()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ConfigureOptions(services);

            return services.BuildServiceProvider(true);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<DiscordSocketClient>()
                    .AddSingleton<CommandService>()
                    .AddSingleton<ExplicitDiscordCommandHandler>()
                    .AddSingleton<ILoggingService, ConsoleLoggingService>()
                    .AddTransient<ITimeProvider, TimeProvider>()
                    .AddTransient<IWeekendInquiryService, WeekendInquiryService>()
                    .AddTransient<BotInformationService>();
        }

        private void ConfigureOptions(IServiceCollection services)
        {
            services.ConfigureAndValidate<ExplicitDiscordCommandOptions>(
                        options => configurationRoot.GetSection(ExplicitDiscordCommandOptions.SectionKey)
                                                    .Bind(options))
                    .ConfigureAndValidate<StringFormatOptions>(
                        options => configurationRoot.GetSection(StringFormatOptions.SectionKey)
                                                    .Bind(options))
                    .ConfigureAndValidate<BotInformation>(
                        options => configurationRoot.GetSection(BotInformation.SectionKey)
                                                    .Bind(options));
        }
    }
}