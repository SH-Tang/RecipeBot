// Copyright (C) 2022 Dennis Tang. All rights reserved.
//
// This file is part of WeekendBot.
//
// WeekendBot is free software: you can redistribute it and/or modify
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
using Discord;
using Discord.Common;
using Discord.Common.Utils;
using Discord.Interactions;
using Discord.WebSocket;
using WeekendBot.Utils;

namespace WeekendBot.Services;

/// <summary>
/// Module containing commands for the recipe.
/// </summary>
public class RecipeInteractionModule : InteractionModuleBase<SocketInteractionContext>
{
    // The ModalResponse instantiates another object to respond, so the state is not preserved between FormatRecipe and the ModalResponse
    private static IAttachment? attachmentArgument;
    private readonly ILoggingService logger;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeInteractionModule"/>.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is <c>null</c>.</exception>
    public RecipeInteractionModule(ILoggingService logger)
    {
        logger.IsNotNull(nameof(logger));
        this.logger = logger;
    }

    [SlashCommand("recipe", "Tell us about your recipe")]
    public async Task FormatRecipe([Summary("image", "The image of the recipe result (optional)")] IAttachment? attachment = null)
    {
        if (attachment != null && !attachment.IsImage())
        {
            await RespondAsync("Attachment must be an image.", ephemeral: true);
            return;
        }

        try
        {
            attachmentArgument = attachment;
            await Context.Interaction.RespondWithModalAsync<RecipeModal>(RecipeModal.ModalId);
        }
        catch (Exception e)
        {
            await logger.LogErrorAsync(e);
            attachmentArgument = null;
        }
    }

    [ModalInteraction(RecipeModal.ModalId)]
    public async Task ModalResponse(RecipeModal modal)
    {
        try
        {
            SocketUser? user = Context.User;
            Embed response = attachmentArgument != null && attachmentArgument.IsImage()
                                 ? RecipeModalResponseService.GetRecipeModalResponse(modal, user, attachmentArgument)
                                 : RecipeModalResponseService.GetRecipeModalResponse(modal, user);
            await RespondAsync(embed: response);
        }
        catch (ModalResponseException e)
        {
            Task[] tasks =
            {
                RespondAsync(e.Message, ephemeral: true),
                logger.LogErrorAsync(e)
            };

            Task.WaitAll(tasks);
        }
        catch (Exception e)
        {
            await logger.LogErrorAsync(e);
        }
        finally
        {
            attachmentArgument = null;
        }
    }
}