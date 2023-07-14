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
/// Module containing commands to interact with author entries.
/// </summary>
public class AuthorInteractionModule : DiscordInteractionModuleBase
{
    private readonly IServiceScopeFactory scopeFactory;

    /// <summary>
    /// Creates a new instance of <see cref="AuthorInteractionModule"/>.
    /// </summary>
    /// <param name="scopeFactory">The <see cref="IServiceScopeFactory"/> to resolve dependencies with.</param>
    /// <param name="logger">The logger to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    public AuthorInteractionModule(IServiceScopeFactory scopeFactory, ILoggingService logger) :
        base(logger)
    {
        scopeFactory.IsNotNull(nameof(scopeFactory));

        this.scopeFactory = scopeFactory;
    }

    [SlashCommand("myuserdata-delete-all", "Deletes all user data")]
    public Task DeleteAuthor()
    {
        return ExecuteControllerAction(async () =>
        {
            using(IServiceScope scope = scopeFactory.CreateScope())
            {
                var controller = scope.ServiceProvider.GetRequiredService<IAuthorController>();
                ControllerResult<string> response = await controller.DeleteAuthorAsync(Context.User);
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

    [SlashCommand("author-delete", "Deletes an author and its associated data")]
    [DefaultMemberPermissions(GuildPermission.Administrator | GuildPermission.ModerateMembers)]
    public Task DeleteAuthorById([Summary("authorId", "The id of the author to delete")] long authorId)
    {
        return ExecuteControllerAction(async () =>
        {
            using(IServiceScope scope = scopeFactory.CreateScope())
            {
                var controller = scope.ServiceProvider.GetRequiredService<IAuthorController>();
                ControllerResult<string> response = await controller.DeleteAuthorAsync(authorId);
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

    [SlashCommand("author-list", "Lists all stored authors in the database")]
    [DefaultMemberPermissions(GuildPermission.Administrator | GuildPermission.ModerateMembers)]
    public Task ListAuthors()
    {
        return ExecuteControllerAction(async () =>
        {
            using(IServiceScope scope = scopeFactory.CreateScope())
            {
                var controller = scope.ServiceProvider.GetRequiredService<IAuthorController>();
                IEnumerable<Task> tasks = await GetTasksAsync(controller.GetAllAuthorsAsync());

                await Task.WhenAll(tasks);
            }
        });
    }
}