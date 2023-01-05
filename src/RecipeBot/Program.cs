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
using Microsoft.Extensions.Hosting;

namespace RecipeBot
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                string configurationFilePath = Path.Combine(AppContext.BaseDirectory, "config.json");
                var application = new RecipeBotApplication(configurationFilePath);

                await application.Run();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(e);
                await Task.Delay(Timeout.Infinite);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            Console.WriteLine(@"Default host builder created for Entity Framework commands");
            return Host.CreateDefaultBuilder();
        }
    }
}