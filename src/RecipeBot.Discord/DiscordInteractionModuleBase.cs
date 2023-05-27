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
using System.Threading.Tasks;
using Common.Utils;
using Discord.Common.Services;
using Discord.Interactions;

namespace RecipeBot.Discord;

/// <summary>
/// Base class for describing Discord interaction modules.
/// </summary>
public abstract class DiscordInteractionModuleBase : InteractionModuleBase<SocketInteractionContext>
{
    protected readonly ILoggingService Logger;

    /// <summary>
    /// Creates a new instance of <see cref="DiscordInteractionModuleBase"/>.
    /// </summary>
    /// <param name="logger">The logger to use</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is <c>null</c>.</exception>
    protected DiscordInteractionModuleBase(ILoggingService logger)
    {
        logger.IsNotNull(nameof(logger));

        Logger = logger;
    }

    protected async Task ExecuteControllerAction(Func<Task> controllerFunc)
    {
        try
        {
            await controllerFunc();
        }
        catch (Exception e)
        {
            Task[] tasks =
            {
                RespondAsync(e.Message, ephemeral: true),
                Task.Run(() => Logger.LogError(e))
            };

            await Task.WhenAll(tasks);
        }
    }
}