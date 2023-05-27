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
using Discord.Common.Services;
using Serilog;

namespace RecipeBot.Services
{
    /// <summary>
    /// Service responsible for logging in the application.
    /// </summary>
    internal class RecipeBotLoggingService : ILoggingService, IDisposable
    {
        /// <summary>
        /// Creates a new instance of the <see cref="RecipeBotLoggingService"/>.
        /// </summary>
        public RecipeBotLoggingService()
        {
            Log.Logger = new LoggerConfiguration()
                         .MinimumLevel.Information()
                         .WriteTo.Console()
                         .WriteTo.File("logs/RecipeBot.txt", rollingInterval: RollingInterval.Day)
                         .CreateLogger();

            LogDebug("Logger successfully created");
        }

        public void Dispose()
        {
            Log.CloseAndFlush();
        }

        public void LogInfo(string message)
        {
            Log.Information(message);
        }

        public void LogError(string message)
        {
            Log.Error(message);
        }

        public void LogError(Exception exception)
        {
            Log.Error(exception, "An error occurred");
        }

        public void LogDebug(string message)
        {
            Log.Debug(message);
        }
    }
}