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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Utils;
using Discord.Common.Services;
using Discord.Interactions;
using RecipeBot.Discord.Controllers;
using RecipeBot.Discord.Properties;

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
    /// <param name="logger">The logger to use.</param>
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
                RespondAsync(string.Format(Resources.InteractionModule_ERROR_0_, e.Message), ephemeral: true),
                Task.Run(() => Logger.LogError(e))
            };

            await Task.WhenAll(tasks);
        }
    }

    protected async Task<IEnumerable<Task>> GetTasksAsync(Task<ControllerResult<IReadOnlyList<string>>> getControllerResultTask)
    {
        ControllerResult<IReadOnlyList<string>> result = await getControllerResultTask;
        if (result.HasError)
        {
            return new[]
            {
                RespondAsync(string.Format(Resources.InteractionModule_ERROR_0_, result.ErrorMessage), ephemeral: true)
            };
        }

        IReadOnlyList<string> messages = result.Result!;
        if (!messages.Any())
        {
            return new[]
            {
                RespondAsync(string.Format(Resources.InteractionModule_ERROR_0_,
                                           Resources.Controller_should_not_have_returned_an_empty_collection_when_querying),
                             ephemeral: true)
            };
        }

        var tasks = new List<Task>
        {
            RespondAsync(messages[0], ephemeral: true)
        };
        for (var i = 1; i < messages.Count; i++)
        {
            tasks.Add(FollowupAsync(messages[i], ephemeral: true));
        }

        return tasks;
    }
}