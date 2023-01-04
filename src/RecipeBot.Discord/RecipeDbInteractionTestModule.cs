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
using Discord.Common;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using RecipeBot.Discord.Data;
using RecipeBot.Discord.Services;
using RecipeBot.Discord.Views;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Factories;
using RecipeBot.Domain.Models;
using RecipeBot.Domain.Repositories;

namespace RecipeBot.Discord;

public class RecipeDbInteractionTestModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILoggingService logger;
    private readonly IServiceScopeFactory scopeFactory;
    private readonly RecipeModelFactory factory;

    public RecipeDbInteractionTestModule(ILoggingService logger,
                                         IRecipeModelCharacterLimitProvider limitProvider,
                                         IServiceScopeFactory scopeFactory)
    {
        logger.IsNotNull(nameof(logger));
        limitProvider.IsNotNull(nameof(limitProvider));
        scopeFactory.IsNotNull(nameof(scopeFactory));

        this.logger = logger;
        this.scopeFactory = scopeFactory;
        factory = new RecipeModelFactory(limitProvider);
    }

    [SlashCommand("recipe-save", "Tests the saving of recipes")]
    public async Task SaveRecipe([Summary("category", "The category the recipe belongs to")] DiscordRecipeCategory category)
    {
        var arguments = CommandArguments.Instance;

        try
        {
            arguments.CategoryArgument = category;
            await Context.Interaction.RespondWithModalAsync<RecipeModal>(RecipeModal.ModalId);
        }
        catch (Exception e)
        {
            await logger.LogErrorAsync(e);
            arguments.ResetArguments();
        }
    }

    [ModalInteraction(RecipeModal.ModalId)]
    public async Task OnModalResponse(RecipeModal modal)
    {
        var arguments = CommandArguments.Instance;

        try
        {
            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IRecipeRepository>();

                DiscordRecipeCategory category = arguments.CategoryArgument;

                SocketUser? user = Context.User;
                var authorData = new AuthorData(user.Username, user.GetAvatarUrl());
                RecipeData recipeData = new RecipeDataBuilder(authorData, category, modal.RecipeTitle!, modal.Ingredients!, modal.CookingSteps!)
                                        .AddNotes(modal.Notes)
                                        .AddTags(modal.Tags)
                                        .Build();
                RecipeModel recipeModel = factory.Create(recipeData);
                await repository.SaveRecipeAsync(recipeModel);
                await RespondAsync("Data saved");
            }
        }
        catch (Exception e)
        {
            Task[] tasks =
            {
                RespondAsync(e.Message, ephemeral: true),
                logger.LogErrorAsync(e)
            };
            
            Task.WaitAll(tasks);
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