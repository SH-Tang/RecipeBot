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
using System.Threading.Tasks;
using Common.Utils;
using Discord;
using Discord.Common.Services;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using RecipeBot.Discord.Controllers;
using RecipeBot.Discord.Properties;

namespace RecipeBot.Discord;

/// <summary>
/// Module containing commands to interact with recipe tag entries.
/// </summary>
public class RecipeTagEntriesInteractionModule : DiscordInteractionModuleBase
{
    private readonly IServiceScopeFactory scopeFactory;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeTagEntriesInteractionModule"/>.
    /// </summary>
    /// <param name="scopeFactory">The <see cref="IServiceScopeFactory"/> to resolve dependencies with.</param>
    /// <param name="logger">The logger to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    public RecipeTagEntriesInteractionModule(IServiceScopeFactory scopeFactory, ILoggingService logger) : base(logger)
    {
        scopeFactory.IsNotNull(nameof(scopeFactory));

        this.scopeFactory = scopeFactory;
    }

    [SlashCommand("tag-list", "Lists all the saved tags")]
    public Task ListTags()
    {
        return ExecuteControllerAction(async () =>
        {
            using(IServiceScope scope = scopeFactory.CreateScope())
            {
                var controller = scope.ServiceProvider.GetRequiredService<IRecipeTagEntriesController>();

                IEnumerable<Task> tasks = await GetTasksAsync(controller.ListAllTagsAsync());
                await Task.WhenAll(tasks);
            }
        });
    }

    [SlashCommand("tag-delete", "Deletes a tag based on the id")]
    [DefaultMemberPermissions(GuildPermission.Administrator | GuildPermission.ModerateMembers)]
    public Task DeleteTag([Summary("TagId", "The id of the tag to delete")] long tagIdToDelete)
    {
        return ExecuteControllerAction(async () =>
        {
            using(IServiceScope scope = scopeFactory.CreateScope())
            {
                var controller = scope.ServiceProvider.GetRequiredService<IRecipeTagEntriesController>();
                ControllerResult<string> response = await controller.DeleteTagAsync(tagIdToDelete);
                if (response.HasError)
                {
                    await RespondAsync(string.Format(Resources.InteractionModule_ERROR_0_, response.ErrorMessage), ephemeral: true);
                }
                else
                {
                    await RespondAsync(response.Result, ephemeral: true);
                }
            }
        });
    }
}