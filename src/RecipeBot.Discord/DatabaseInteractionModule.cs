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
using System.Linq;
using System.Threading.Tasks;
using Common.Utils;
using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecipeBot.Persistence;
using RecipeBot.Persistence.Entities;

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
    public async Task SaveData([Summary("title", "The title of the recipe")] string recipeTitle,
                               [Summary("author", "The name of the author")]
                               string authorName)
    {
        using (IServiceScope serviceScope = scopeFactory.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<RecipeBotDbContext>();

            // Find author and if found add it
            AuthorEntity authorEntity = context.AuthorEntities.FirstOrDefault(a => a.Name == authorName) ?? new AuthorEntity
            {
                Name = authorName
            };

            context.RecipeEntities.Add(new RecipeEntity
            {
                Title = recipeTitle,
                Author = authorEntity
            });

            try
            {
                await context.SaveChangesAsync();
                await Context.Interaction.RespondAsync($"Saving following data to the database: {recipeTitle}, {authorName}");
            }
            catch (Exception ex)
            {
                await Context.Interaction.RespondAsync($"Saving to database failed: {ex.Message}");
            }
        }
    }

    [SlashCommand("recipe-get", "Get the recipe data")]
    public async Task GetData([Summary("ID", "The id pf the recipe")] int id)
    {
        using (IServiceScope serviceScope = scopeFactory.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<RecipeBotDbContext>();

            RecipeEntity? recipeData = context.RecipeEntities
                                              .Include(e => e.Author)
                                              .FirstOrDefault(e => e.Id == id);

            if (recipeData == null)
            {
                await Context.Interaction.RespondAsync($"Data with ID '{id}' not found.");
            }
            else
            {
                await Context.Interaction.RespondAsync($"Retrieved data: {recipeData.Title} with author {recipeData.Author.Name}.");
            }
        }
    }

    [DefaultMemberPermissions(GuildPermission.Administrator | GuildPermission.ManageGuild)] // Permissions to prevent entities other than entities with managing server roles and administrative permissions from using the command
    [SlashCommand("recipe-delete", "Delete the recipe data")]
    public async Task DeleteData([Summary("ID", "The id of the recipe to delete")] int id)
    {
        using (IServiceScope serviceScope = scopeFactory.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<RecipeBotDbContext>();

            RecipeEntity? recipeData = context.RecipeEntities.FirstOrDefault(e => e.Id == id);

            if (recipeData == null)
            {
                await Context.Interaction.RespondAsync($"Data with ID '{id}' not found.");
            }
            else
            {
                try
                {
                    context.Remove(recipeData);
                    await context.SaveChangesAsync();
                    await Context.Interaction.RespondAsync($"Removed following data from the database: {recipeData.Title}");
                }
                catch (Exception ex)
                {
                    await Context.Interaction.RespondAsync($"Saving to database failed: {ex.Message}");
                }
            }
        }
    }
}