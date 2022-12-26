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
using RecipeBot.Discord.Data;
using RecipeBot.Discord.Services;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Factories;
using RecipeBot.Domain.Repositories;

namespace RecipeBot.Discord;

public class DatabaseInteractionModule : InteractionModuleBase<SocketInteractionContext>
{
    private const string authorImageUrl = @"https://static.vecteezy.com/system/resources/previews/003/725/245/non_2x/cat-cute-love-noodles-free-vector.jpg";
    private readonly IServiceScopeFactory scopeFactory;
    private readonly RecipeModelFactory factory;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeInteractionModule"/>.
    /// </summary>
    /// <param name="scopeFactory">The <see cref="IServiceScopeFactory"/> for creating services within scope.</param>
    /// <param name="limitProvider">The <see cref="IRecipeModelCharacterLimitProvider"/> to retrieve the character limits from.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    public DatabaseInteractionModule(IServiceScopeFactory scopeFactory, IRecipeModelCharacterLimitProvider limitProvider)
    {
        scopeFactory.IsNotNull(nameof(scopeFactory));
        limitProvider.IsNotNull(nameof(limitProvider));

        this.scopeFactory = scopeFactory;
        factory = new RecipeModelFactory(limitProvider);
    }

    [SlashCommand("recipe-save", "Save recipe data")]
    public async Task SaveData([Summary("title", "The title of the recipe")] string recipeTitle,
                               [Summary("author", "The name of the author")]
                               string authorName)
    {
        using (IServiceScope serviceScope = scopeFactory.CreateScope())
        {
            var repository = serviceScope.ServiceProvider.GetRequiredService<IRecipeRepository>();

            var authorData = new AuthorData(authorName, authorImageUrl);
            RecipeData recipeData = new RecipeDataBuilder(authorData, DiscordRecipeCategory.Other, recipeTitle,
                                                          "Ingredients don't matter", "Cooking steps don't matter").Build();

            await repository.SaveRecipeAsync(factory.Create(recipeData));
            await Context.Interaction.RespondAsync($"Saving following data to the database: {recipeTitle}, {authorName}");
        }
    }

    [SlashCommand("recipe-get", "Get the recipe data")]
    public async Task GetData([Summary("ID", "The id of the recipe")] int id)
    {
        using (IServiceScope serviceScope = scopeFactory.CreateScope())
        {
            var repository = serviceScope.ServiceProvider.GetRequiredService<IRecipeRepository>();

            RecipeData? recipeData = await repository.GetRecipeByIdAsync(id);
            if (recipeData == null)
            {
                await Context.Interaction.RespondAsync($"Data with ID '{id}' not found.");
            }
            else
            {
                await Context.Interaction.RespondAsync($"Retrieved data: {recipeData.RecipeTitle} with author {recipeData.AuthorData.AuthorName}.");
            }
        }
    }

    [DefaultMemberPermissions(GuildPermission.Administrator | GuildPermission.ManageGuild)] // Permissions to prevent entities other than entities with managing server roles and administrative permissions from using the command
    [SlashCommand("recipe-delete", "Delete the recipe data")]
    public async Task DeleteData([Summary("ID", "The id of the recipe to delete")] int id)
    {
        using (IServiceScope serviceScope = scopeFactory.CreateScope())
        {
            var repository = serviceScope.ServiceProvider.GetRequiredService<IRecipeRepository>();
            RecipeData? recipeData = await repository.DeleteRecipeAsync(id);
            if (recipeData == null)
            {
                await Context.Interaction.RespondAsync($"ERROR: Recipe with id '{id}' not found.");
            }
            else
            {
                await Context.Interaction.RespondAsync($"Removed following data from the database: {recipeData.RecipeTitle}.");
            }
        }
    }
}