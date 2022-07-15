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
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace WeekendBot
{
    internal class Program
    {
        private DiscordSocketClient discordClient;
        private IConfiguration configuration;

        public static Task Main(string[] args)
        {
            return new Program().MainAsync();
        }

        public Program()
        {
            var builder = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("config.json");

            configuration = builder.Build();
        }

        public async Task MainAsync()
        {
            discordClient = new DiscordSocketClient();
            discordClient.Log += Log;

            string token = configuration["Token"];
            await discordClient.LoginAsync(TokenType.Bot, token);
            await discordClient.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}