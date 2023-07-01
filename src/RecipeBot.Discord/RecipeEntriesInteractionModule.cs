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
using Discord;
using Discord.Common.Services;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using RecipeBot.Discord.Controllers;
using RecipeBot.Discord.Data;
using RecipeBot.Discord.Properties;

namespace RecipeBot.Discord;

/// <summary>
/// Module containing commands to interact with recipe entries.
/// </summary>
public class RecipeEntriesInteractionModule : DiscordInteractionModuleBase
{
    private readonly IServiceScopeFactory scopeFactory;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeEntriesInteractionModule"/>.
    /// </summary>
    /// <param name="scopeFactory">The <see cref="IServiceScopeFactory"/> to resolve dependencies with.</param>
    /// <param name="logger">The logger to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    public RecipeEntriesInteractionModule(IServiceScopeFactory scopeFactory, ILoggingService logger) : base(logger)
    {
        scopeFactory.IsNotNull(nameof(scopeFactory));
        logger.IsNotNull(nameof(logger));

        this.scopeFactory = scopeFactory;
    }

    [SlashCommand("recipe-list", "Lists all the saved user recipes")]
    public async Task GetAllRecipes()
    {
        await ExecuteControllerAction(async () => await GetRecipeResponseTasks(c => c.GetAllRecipesAsync()));
    }

    [SlashCommand("recipe-list-by-category", "Lists all the saved user recipes filtered by category")]
    public async Task GetAllRecipeByCategory([Summary("category", "The category to filter the recipes with")] DiscordRecipeCategory category)
    {
        await ExecuteControllerAction(async () => await GetRecipeResponseTasks(c => c.GetAllRecipesByCategoryAsync(category)));
    }

    [SlashCommand("recipe-list-by-tag-id", "Lists all the saved user recipes filtered by tag id")]
    public async Task GetAllRecipeByTagId([Summary("tagId", "The tag id to filter the recipes with")] long tagId)
    {
        await ExecuteControllerAction(async () => await GetRecipeResponseTasks(c => c.GetAllRecipesByTagIdAsync(tagId)));
    }

    [SlashCommand("recipe-list-by-tag", "Lists all the saved user recipes filtered by tag")]
    public async Task GetAllRecipeByTag([Summary("tag", "The tag to filter the recipes with")] string tag)
    {
        await ExecuteControllerAction(async () => await GetRecipeResponseTasks(c => c.GetAllRecipesByTagAsync(tag)));
    }

    [SlashCommand("myrecipes-list", "Lists all your saved user recipes")]
    public async Task GetAllRecipeByUser()
    {
        await ExecuteControllerAction(async () => await GetRecipeResponseTasks(c => c.GetAllRecipesByUserAsync(Context.User)));
    }

    private async Task GetRecipeResponseTasks(Func<IRecipeEntriesController, Task<ControllerResult<IReadOnlyList<string>>>> getControllerResultTaskFunc)
    {
        using(IServiceScope scope = scopeFactory.CreateScope())
        {
            var controller = scope.ServiceProvider.GetRequiredService<IRecipeEntriesController>();
            IEnumerable<Task> tasks = await GetTasksAsync(getControllerResultTaskFunc(controller));
            await Task.WhenAll(tasks);
        }
    }
}