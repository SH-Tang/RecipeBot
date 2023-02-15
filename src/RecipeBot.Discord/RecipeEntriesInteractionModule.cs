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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Utils;
using Discord.Common;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using RecipeBot.Discord.Controllers;
using RecipeBot.Discord.Data;
using RecipeBot.Discord.Properties;

namespace RecipeBot.Discord;

/// <summary>
/// Module containing commands to interact with recipe entries.
/// </summary>
public class RecipeEntriesInteractionModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILoggingService logger;
    private readonly IServiceScopeFactory scopeFactory;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeEntriesInteractionModule"/>.
    /// </summary>
    /// <param name="scopeFactory">The <see cref="IServiceScopeFactory"/> to resolve dependencies with.</param>
    /// <param name="logger">The logger to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    public RecipeEntriesInteractionModule(IServiceScopeFactory scopeFactory, ILoggingService logger)
    {
        scopeFactory.IsNotNull(nameof(scopeFactory));
        logger.IsNotNull(nameof(logger));

        this.scopeFactory = scopeFactory;
        this.logger = logger;
    }

    [SlashCommand("recipe-list", "Lists all the saved user recipes")]
    public async Task GetAllRecipes()
    {
        await Task.WhenAll(GetRecipeResponseTasks(c => c.GetAllRecipesAsync()));
    }

    [SlashCommand("recipe-list-by-category", "Lists all the saved user recipes filtered by category")]
    public async Task GetAllRecipeByCategory([Summary("category", "The category to filter the recipes with")] DiscordRecipeCategory category)
    {
        await Task.WhenAll(GetRecipeResponseTasks(c => c.GetAllRecipesByCategoryAsync(category)));
    }

    [SlashCommand("recipe-list-by-tag-id", "Lists all the saved user recipes filtered by tag id")]
    public async Task GetAllRecipeByTagId([Summary("tagId", "The tag id to filter the recipes with")] long tagId)
    {
        await Task.WhenAll(GetRecipeResponseTasks(c => c.GetAllRecipesByTagIdAsync(tagId)));
    }

    [SlashCommand("recipe-list-by-tag", "Lists all the saved user recipes filtered by tag")]
    public async Task GetAllRecipeByTag([Summary("tag", "The tag to filter the recipes with")] string tag)
    {
        await Task.WhenAll(GetRecipeResponseTasks(c => c.GetAllRecipesByTagAsync(tag)));
    }

    private IEnumerable<Task> GetRecipeResponseTasks(Func<IRecipeEntriesController, Task<ControllerResult<IReadOnlyList<string>>>> getControllerResultTaskFunc)
    {
        try
        {
            using(IServiceScope scope = scopeFactory.CreateScope())
            {
                var controller = scope.ServiceProvider.GetRequiredService<IRecipeEntriesController>();

                Task<ControllerResult<IReadOnlyList<string>>> getRecipeEntriesTask = getControllerResultTaskFunc(controller);

                return new[]
                {
                    getRecipeEntriesTask.ContinueWith(GetTasksFromControllerResultAsync)
                };
            }
        }
        catch (Exception e)
        {
            return new[]
            {
                RespondAsync(e.Message, ephemeral: true),
                logger.LogErrorAsync(e)
            };
        }
    }

    private async Task<IEnumerable<Task>> GetTasksFromControllerResultAsync(Task<ControllerResult<IReadOnlyList<string>>> controllerResultTask)
    {
        ControllerResult<IReadOnlyList<string>> result = await controllerResultTask;
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