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

namespace RecipeBot.Discord;

public class DatabaseInteractionModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILoggingService logger;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeInteractionModule"/>.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    public DatabaseInteractionModule(ILoggingService logger)
    {
        logger.IsNotNull(nameof(logger));
        this.logger = logger;
    }

    [SlashCommand("recipe-save", "Save recipe data")]
    public async Task SaveData([Summary("title", "The title of the recipe")] string recipeTitle,
                                   [Summary("author", "The name of the author")] string authorName)
    {
        await Context.Interaction.RespondAsync($"Saving following data to the database: {recipeTitle}, {authorName}");
    }
}