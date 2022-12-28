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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Utils;
using RecipeBot.Discord.Data;
using RecipeBot.Discord.Services;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Factories;
using RecipeBot.Domain.Repositories;
using RecipeBot.Domain.Repositories.DTO;

namespace RecipeBot.Discord.Controllers;

/// <summary>
/// Controller for determining the recipe responses.
/// </summary>
public class RecipeController : IRecipeController
{
    private const string authorImageUrl = @"https://static.vecteezy.com/system/resources/previews/003/725/245/non_2x/cat-cute-love-noodles-free-vector.jpg";

    private readonly IRecipeRepository repository;
    private readonly RecipeModelFactory factory;

    public RecipeController(IRecipeRepository repository, IRecipeModelCharacterLimitProvider limitProvider)
    {
        repository.IsNotNull(nameof(repository));
        limitProvider.IsNotNull(nameof(limitProvider));

        this.repository = repository;
        factory = new RecipeModelFactory(limitProvider);
    }

    public async Task<string> SaveRecipe(string recipeTitle, string authorName)
    {
        var authorData = new AuthorData(authorName, authorImageUrl);
        RecipeData recipeData = new RecipeDataBuilder(authorData, DiscordRecipeCategory.Other, recipeTitle,
                                                      "Ingredients don't matter", "Cooking steps don't matter").Build();

        await repository.SaveRecipeAsync(factory.Create(recipeData));
        return ($"Saving following data to the database: {recipeTitle}, {authorName}");
    }

    public async Task<string> FindRecipeAsync(int id)
    {
        RecipeDto? recipeDto = await repository.GetRecipeByIdAsync(id);
        return recipeDto == null
                   ? $"Data with ID '{id}' not found."
                   : $"Retrieved data: {recipeDto.Title} with author {recipeDto.Author.Name}.";
    }

    public async Task<string> GetAllRecipesAsync()
    {
        IEnumerable<RecipeDto> recipes = await repository.GetAllRecipes();

        string header = $"{"Id",-3} {"Title",-50} {"Author",-50}";
        string entries = string.Join($"{Environment.NewLine}", recipes.Select(r => $"{r.Id,-3} {r.Title,-50} {r.Author.Name,-50}"));
        return header + Environment.NewLine + entries;
    }

    public async Task<string> DeleteRecipeAsync(int id)
    {
        RecipeDto? recipeData = await repository.DeleteRecipeAsync(id);
        return recipeData == null 
                   ? $"ERROR: Recipe with id '{id}' not found." 
                   : $"Removed following data from the database: {recipeData.Title}.";
    }
}