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
using Discord;
using Discord.Common;
using Discord.Common.Utils;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using RecipeBot.Discord.Controllers;
using RecipeBot.Discord.Data;
using RecipeBot.Discord.Properties;
using RecipeBot.Discord.Views;

namespace RecipeBot.Discord;

/// <summary>
/// Module containing commands for the recipe.
/// </summary>
public class RecipeInteractionModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly ILoggingService logger;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeInteractionModule"/>.
    /// </summary>
    /// <param name="scopeFactory">The <see cref="IServiceScopeFactory"/> to resolve dependencies with.</param>
    /// <param name="logger">The logger to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="scopeFactory"/> is <c>null</c>.</exception>
    public RecipeInteractionModule(IServiceScopeFactory scopeFactory,
                                   ILoggingService logger)
    {
        scopeFactory.IsNotNull(nameof(scopeFactory));
        logger.IsNotNull(nameof(logger));

        this.scopeFactory = scopeFactory;
        this.logger = logger;
    }

    [SlashCommand("recipe", "Formats and stores an user recipe")]
    public async Task FormatRecipe([Summary("category", "The category the recipe belongs to")] DiscordRecipeCategory category,
                                   [Summary("image", "The image of the recipe result (optional)")]
                                   IAttachment? attachment = null)
    {
        if (attachment != null && !attachment.IsImage())
        {
            await RespondAsync(Resources.Attachment_must_be_an_image, ephemeral: true);
            return;
        }

        var arguments = CommandArguments.Instance;

        try
        {
            arguments.AttachmentArgument = attachment;
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
            IAttachment? attachment = arguments.AttachmentArgument;
            DiscordRecipeCategory category = arguments.CategoryArgument;

            SocketUser? user = Context.User;

            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                var controller = scope.ServiceProvider.GetRequiredService<IRecipeController>();
                ControllerResult<Embed> response = await controller.SaveRecipeAsync(modal, user, category, attachment);

                if (response.HasError)
                {
                    await RespondAsync(string.Format(Resources.RecipeInteractionModule_ERROR_0_, response.ErrorMessage), ephemeral: true);
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
                logger.LogErrorAsync(e)
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
        /// Gets ors sets the <see cref="IAttachment"/>.
        /// </summary>
        public IAttachment? AttachmentArgument { get; set; }

        /// <summary>
        /// Resets the arguments to default arguments.
        /// </summary>
        public void ResetArguments()
        {
            AttachmentArgument = null;
            CategoryArgument = (DiscordRecipeCategory)(-1);
        }
    }
}