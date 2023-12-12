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
using RecipeBot.Discord.Controllers;

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
    public Task ParseWebRecipe([Summary("WebRecipe", "The website of the recipe to parse")] string webRecipeUrl, string title = "Alternative title")
    {
        return ExecuteControllerAction(async () =>
        {
            ControllerResult<Embed> result = await controller.ParseRecipe(webRecipeUrl, Context.User, title);
            await RespondAsync(embed: result.Result);
        });
    }
}