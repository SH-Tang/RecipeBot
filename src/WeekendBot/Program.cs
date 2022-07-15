// Copyright (C) 2022 Dennis Tang. All rights reserved.
//
// This file is part of WeekendBot.
//
// Balanced Field Length is free software: you can redistribute it and/or modify
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
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeekendBot.Services;

namespace WeekendBot
{
    internal class Program
    {
        private readonly IConfiguration configuration;
        private DiscordSocketClient discordClient;

        private Program()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("config.json");

            configuration = builder.Build();
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
            discordClient.Log += LogAsync;

            string token = configuration["Token"];
            await discordClient.LoginAsync(TokenType.Bot, token);
            await discordClient.StartAsync();
        }

        private static async Task ConfigureCommandHandlingService(IServiceProvider services)
        {
            var commandHandlingService = services.GetRequiredService<CommandHandlingService>();
            await commandHandlingService.InitializeServiceAsync();
        }

        private static async Task ConfigureCommandService(IServiceProvider services)
        {
            var commandService = services.GetRequiredService<CommandService>();
            commandService.Log += LogAsync;

            await Task.CompletedTask;
        }

        private static Task LogAsync(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private static ServiceProvider GetServiceProvider()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .BuildServiceProvider();
        }
    }
}