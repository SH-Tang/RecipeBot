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
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using RecipeBot.Discord.Controllers;

namespace RecipeBot.Discord;

public class DatabaseInteractionModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IServiceScopeFactory scopeFactory;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeInteractionModule"/>.
    /// </summary>
    /// <param name="scopeFactory">The <see cref="IServiceScopeFactory"/> for creating services within scope.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    public DatabaseInteractionModule(IServiceScopeFactory scopeFactory)
    {
        scopeFactory.IsNotNull(nameof(scopeFactory));

        this.scopeFactory = scopeFactory;
    }

    [SlashCommand("recipe-save", "Save recipe data")]
    public async Task SaveRecipe([Summary("title", "The title of the recipe")] string recipeTitle,
                                 [Summary("author", "The name of the author")]
                                 string authorName)
    {
        using (IServiceScope serviceScope = scopeFactory.CreateScope())
        {
            var controller = serviceScope.ServiceProvider.GetRequiredService<IRecipeController>();

            string message = await controller.SaveRecipe(recipeTitle, authorName);
            await Context.Interaction.RespondAsync(message);
        }
    }

    [SlashCommand("recipe-get", "Gets recipe data")]
    public async Task GetRecipe([Summary("ID", "The id of the recipe")] int? id = null)
    {
        using (IServiceScope serviceScope = scopeFactory.CreateScope())
        {
            var controller = serviceScope.ServiceProvider.GetRequiredService<IRecipeController>();
            if (id == null)
            {
                string recipes = await controller.GetAllRecipesAsync();

                await Context.Interaction.RespondAsync(Format.Code(recipes), ephemeral: true);
                await Context.Interaction.FollowupAsync(Format.Code(recipes), ephemeral: true); // Follow up only possible after RespondAsync acknowledges the interaction
            }
            else
            {
                string recipe = await controller.FindRecipeAsync(id.Value);
                await Context.Interaction.RespondAsync(recipe);
            }
        }
    }

    [DefaultMemberPermissions(GuildPermission.Administrator | GuildPermission.ManageGuild)] // Permissions to prevent entities other than entities with managing server roles and administrative permissions from using the command
    [SlashCommand("recipe-delete", "Delete the recipe data")]
    public async Task DeleteRecipe([Summary("ID", "The id of the recipe to delete")] int id)
    {
        using (IServiceScope serviceScope = scopeFactory.CreateScope())
        {
            var controller = serviceScope.ServiceProvider.GetRequiredService<IRecipeController>();

            string deletedRecipe = await controller.DeleteRecipeAsync(id);
            await Context.Interaction.RespondAsync(deletedRecipe);
        }
    }
}