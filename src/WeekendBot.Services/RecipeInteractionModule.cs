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

using System.Threading.Tasks;
using Discord;
using Discord.Common;
using Discord.Common.Utils;
using Discord.Interactions;
using Discord.WebSocket;

namespace WeekendBot.Services;

/// <summary>
/// Module containing commands for the recipe.
/// </summary>
public class RecipeInteractionModule : InteractionModuleBase<SocketInteractionContext>
{
    private static IAttachment? attachmentArgument; // Because the FormatRecipe works on a different context, attachment must be set as a global variable to pass through

    [SlashCommand("recipe", "Tell us about your recipe")]
    public async Task FormatRecipe([Summary("image", "The image of the recipe result (optional)")] IAttachment? attachment = null)
    {
        attachmentArgument = attachment;
        try
        {
            await Context.Interaction.RespondWithModalAsync<RecipeModal>(RecipeModal.ModalId);
        }
        catch
        {
            attachmentArgument = null;
        }
    }

    [ModalInteraction(RecipeModal.ModalId)]
    public async Task ModalResponse(RecipeModal modal)
    {
        SocketUser? user = Context.User;
        var authorData = new AuthorData(user.Username, user.GetAvatarUrl());

        RecipeDataBuilder recipeDataBuilder = new RecipeDataBuilder(authorData, modal.RecipeTitle, modal.Ingredients, modal.CookingSteps)
            .AddNotes(modal.Notes);
        if (attachmentArgument != null && attachmentArgument.IsImage()) // TODO: Generate error response when attachment is not an image.
        {
            recipeDataBuilder.AddImage(attachmentArgument);
        }

        Embed embed = RecipeEmbedFactory.Create(recipeDataBuilder.Build());

        try
        {
            await RespondAsync(embed: embed);
        }
        finally
        {
            attachmentArgument = null;
        }
    }
}