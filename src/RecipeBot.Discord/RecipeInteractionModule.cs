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
using System.Threading.Tasks;
using Common.Utils;
using Discord;
using Discord.Common.Services;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using RecipeBot.Discord.Controllers;
using RecipeBot.Discord.Data;
using RecipeBot.Discord.Properties;
using RecipeBot.Discord.Views;

namespace RecipeBot.Discord;

/// <summary>
/// Module containing commands to interact with individual recipes.
/// </summary>
public class RecipeInteractionModule : DiscordInteractionModuleBase
{
    private readonly IServiceScopeFactory scopeFactory;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeInteractionModule"/>.
    /// </summary>
    /// <param name="scopeFactory">The <see cref="IServiceScopeFactory"/> to resolve dependencies with.</param>
    /// <param name="logger">The logger to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    public RecipeInteractionModule(IServiceScopeFactory scopeFactory, ILoggingService logger) : base(logger)
    {
        scopeFactory.IsNotNull(nameof(scopeFactory));

        this.scopeFactory = scopeFactory;
    }

    [SlashCommand("recipe-get", "Gets a recipe based on the id")]
    public Task GetRecipe([Summary("RecipeId", "The id of the recipe to retrieve")] long recipeIdToRetrieve)
    {
        return ExecuteControllerAction(async () =>
        {
            using(IServiceScope scope = scopeFactory.CreateScope())
            {
                var controller = scope.ServiceProvider.GetRequiredService<IRecipeController>();
                ControllerResult<Embed> response = await controller.GetRecipeAsync(recipeIdToRetrieve);
                if (response.HasError)
                {
                    await RespondAsync(string.Format(Resources.InteractionModule_ERROR_0_, response.ErrorMessage), ephemeral: true);
                }
                else
                {
                    await RespondAsync(embed: response.Result, ephemeral: true);
                }
            }
        });
    }

    [SlashCommand("recipe-delete", "Deletes a recipe based on the id")]
    [DefaultMemberPermissions(GuildPermission.Administrator | GuildPermission.ModerateMembers)]
    public Task DeleteRecipe([Summary("RecipeId", "The id of the recipe to delete")] long recipeIdToDelete)
    {
        return ExecuteControllerAction(async () =>
        {
            using(IServiceScope scope = scopeFactory.CreateScope())
            {
                var controller = scope.ServiceProvider.GetRequiredService<IRecipeController>();
                ControllerResult<string> response = await controller.DeleteRecipeAsync(recipeIdToDelete);
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

    [SlashCommand("recipe", "Formats and stores an user recipe")]
    public Task SaveRecipe([Summary("category", "The category the recipe belongs to")] DiscordRecipeCategory category)
    {
        var arguments = CommandArguments.Instance;

        try
        {
            arguments.CategoryArgument = category;

            return Context.Interaction.RespondWithModalAsync<RecipeModal>(RecipeModal.ModalId);
        }
        catch (Exception e)
        {
            Logger.LogError(e);
            arguments.ResetArguments();

            return RespondAsync(string.Format(Resources.InteractionModule_ERROR_0_, e.Message), ephemeral: true);
        }
    }

    [SlashCommand("myrecipes-delete", "Deletes an user recipe based on the id")]
    public Task DeleteMyRecipe([Summary("RecipeId", "The id of the recipe to delete")] long recipeIdToDelete)
    {
        return ExecuteControllerAction(async () =>
        {
            using(IServiceScope scope = scopeFactory.CreateScope())
            {
                var controller = scope.ServiceProvider.GetRequiredService<IRecipeController>();
                ControllerResult<string> response = await controller.DeleteRecipeAsync(recipeIdToDelete, Context.User);
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

    [ModalInteraction(RecipeModal.ModalId)]
    public async Task OnModalResponse(RecipeModal modal)
    {
        var arguments = CommandArguments.Instance;

        try
        {
            DiscordRecipeCategory category = arguments.CategoryArgument;

            SocketUser? user = Context.User;

            using(IServiceScope scope = scopeFactory.CreateScope())
            {
                var controller = scope.ServiceProvider.GetRequiredService<IRecipeController>();
                ControllerResult<Embed> response = await controller.SaveRecipeAsync(modal, user, category);

                if (response.HasError)
                {
                    await RespondAsync(string.Format(Resources.InteractionModule_ERROR_0_, response.ErrorMessage), ephemeral: true);
                }
                else
                {
                    await RespondAsync(embed: response.Result);
                }
            }
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
        finally
        {
            arguments.ResetArguments();
        }
    }

    /// <summary>
    /// Class representing the arguments that have been passed.
    /// </summary>
    /// <remarks>Because the OnModalResponse instantiates another object to respond,
    /// the state is not preserved between FormatRecipe and the OnModalResponse. Therefore this singleton is necessary to pass
    /// the arguments between the different instances.</remarks>
    private sealed class CommandArguments
    {
        private static CommandArguments? instance;

        private CommandArguments() {}

        /// <summary>
        /// Gets an instance of <see cref="CommandArguments"/>.
        /// </summary>
        public static CommandArguments Instance => instance ?? (instance = new CommandArguments());

        /// <summary>
        /// Gets or sets the <see cref="DiscordRecipeCategory"/>.
        /// </summary>
        public DiscordRecipeCategory CategoryArgument { get; set; }

        /// <summary>
        /// Resets the arguments to default arguments.
        /// </summary>
        public void ResetArguments()
        {
            CategoryArgument = (DiscordRecipeCategory)(-1);
        }
    }
}