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
using Microsoft.Extensions.DependencyInjection;
using RecipeBot.Discord.Controllers;
using RecipeBot.Discord.Properties;

namespace RecipeBot.Discord;

/// <summary>
/// Module containing commands to interact with author entries.
/// </summary>
public class AuthorInteractionModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly ILoggingService logger;

    /// <summary>
    /// Creates a new instance of <see cref="AuthorInteractionModule"/>.
    /// </summary>
    /// <param name="scopeFactory">The <see cref="IServiceScopeFactory"/> to resolve dependencies with.</param>
    /// <param name="logger">The logger to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    public AuthorInteractionModule(IServiceScopeFactory scopeFactory, ILoggingService logger)
    {
        scopeFactory.IsNotNull(nameof(scopeFactory));
        logger.IsNotNull(nameof(logger));

        this.scopeFactory = scopeFactory;
        this.logger = logger;
    }

    [SlashCommand("myuserdata-delete-all", "Deletes all user data")]
    public async Task DeleteAuthor()
    {
        try
        {
            using (IServiceScope scope = scopeFactory.CreateScope())
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
        }
        catch (Exception e)
        {
            Task[] tasks =
            {
                RespondAsync(e.Message, ephemeral: true),
                logger.LogError(e)
            };

            await Task.WhenAll(tasks);
        }
    }
}