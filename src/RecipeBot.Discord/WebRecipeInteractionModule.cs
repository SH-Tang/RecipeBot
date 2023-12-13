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
using Discord;
using Discord.Common.Services;
using Discord.Interactions;
using Discord.WebSocket;
using RecipeBot.Discord.Controllers;
using RecipeBot.Discord.Data;
using RecipeBot.Discord.Properties;
using RecipeBot.Discord.Views;

namespace RecipeBot.Discord;

/// <summary>
/// Module containing commands to interact with web recipes.
/// </summary>
public class WebRecipeInteractionModule : DiscordInteractionModuleBase
{
    private readonly IWebRecipeController controller;

    /// <summary>
    /// Creates a new instance of <see cref="WebRecipeInteractionModule"/>.
    /// </summary>
    /// <param name="controller">The provider to retrieve html content with.</param>
    /// <param name="logger">The logger to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
    public WebRecipeInteractionModule(IWebRecipeController controller, ILoggingService logger) : base(logger)
    {
        this.controller = controller;
    }

    [SlashCommand("webrecipe-parse", "Parses a web recipe.")]
    public Task ParseWebRecipe(
        [Summary("category", "The category the recipe belongs to")]
        DiscordRecipeCategory category,
        [Summary("WebRecipe", "The website of the recipe to parse")]
        string webRecipeUrl)
    {
        var arguments = CommandArguments.Instance;

        try
        {
            arguments.CategoryArgument = category;
            arguments.WebRecipeUrl = webRecipeUrl;

            return Context.Interaction.RespondWithModalAsync<WebRecipeModal>(WebRecipeModal.ModalId);
        }
        catch (Exception e)
        {
            Logger.LogError(e);
            arguments.ResetArguments();

            return RespondAsync(string.Format(Resources.InteractionModule_ERROR_0_, e.Message), ephemeral: true);
        }
    }

    [ModalInteraction(WebRecipeModal.ModalId)]
    public async Task OnModalResponse(WebRecipeModal modal)
    {
        var arguments = CommandArguments.Instance;
        string? webRecipeUrl = arguments.WebRecipeUrl;
        if (webRecipeUrl == null)
        {
            // TODO: Make logical error message
            await RespondAsync("Error occurred");
            return;
        }

        try
        {
            DiscordRecipeCategory category = arguments.CategoryArgument;

            SocketUser? user = Context.User;
            ControllerResult<Embed> response = await controller.ParseWebRecipeAsync(webRecipeUrl, modal, user, category);

            if (response.HasError)
            {
                await RespondAsync(string.Format(Resources.InteractionModule_ERROR_0_, response.ErrorMessage), ephemeral: true);
            }
            else
            {
                await RespondAsync(embed: response.Result);
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
        /// Gets or sets the url of the web recipe.
        /// </summary>
        public string? WebRecipeUrl { get; set; }

        /// <summary>
        /// Resets the arguments to default arguments.
        /// </summary>
        public void ResetArguments()
        {
            CategoryArgument = (DiscordRecipeCategory)(-1);
            WebRecipeUrl = null;
        }
    }
}