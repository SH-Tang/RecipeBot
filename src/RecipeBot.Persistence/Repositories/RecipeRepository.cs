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
using Microsoft.EntityFrameworkCore;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Models;
using RecipeBot.Domain.Repositories;
using RecipeBot.Persistence.Entities;

namespace RecipeBot.Persistence.Repositories;

/// <summary>
/// A Entity Framework Core implementation of <see cref="IRecipeRepository"/>.
/// </summary>
public class RecipeRepository : IRecipeRepository
{
    private const string authorImageUrl = @"https://static.vecteezy.com/system/resources/previews/003/725/245/non_2x/cat-cute-love-noodles-free-vector.jpg";
    private readonly RecipeBotDbContext context;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeRepository"/>.
    /// </summary>
    /// <param name="context">The <see cref="RecipeBotDbContext"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <c>null</c>.</exception>
    public RecipeRepository(RecipeBotDbContext context)
    {
        context.IsNotNull(nameof(context));
        this.context = context;
    }

    public async Task SaveRecipeAsync(RecipeModel recipe)
    {
        recipe.IsNotNull(nameof(recipe));

        AuthorEntity? authorEntity = await FindAuthorAsync(recipe.Author);
        if (authorEntity == null)
        {
            AuthorModel authorModel = recipe.Author;
            authorEntity = new AuthorEntity
            {
                Name = authorModel.AuthorName
            };
        }

        var recipeEntity = new RecipeEntity
        {
            Title = recipe.Title,
            Author = authorEntity
        };

        context.RecipeEntities.Add(recipeEntity);
        await context.SaveChangesAsync(); // Might throw exceptions, catch the lower level exceptions
    }

    public async Task<RecipeData?> DeleteRecipeAsync(int id)
    {
        RecipeEntity? entity = await context.RecipeEntities
                                            .Include(e => e.Author)
                                            .SingleOrDefaultAsync(r => r.Id == id);
        if (entity == null)
        {
            return null;
        }

        context.RecipeEntities.Remove(entity);
        await context.SaveChangesAsync();

        var authorData = new AuthorData(entity.Author.Name!, authorImageUrl);
        return new RecipeData(authorData, RecipeCategory.Other, entity.Title!, "RecipeIngredients", "CookingSteps");
    }

    public async Task<RecipeData?> GetRecipeByIdAsync(int id)
    {
        RecipeEntity? entity = await context.RecipeEntities
                                            .Include(e => e.Author)
                                            .SingleOrDefaultAsync(r => r.Id == id);

        if (entity == null)
        {
            return null;
        }

        var authorData = new AuthorData(entity.Author.Name!, authorImageUrl);
        return new RecipeData(authorData, RecipeCategory.Other, entity.Title!, "RecipeIngredients", "CookingSteps");
    }

    private Task<AuthorEntity?> FindAuthorAsync(AuthorModel author)
    {
        return context.AuthorEntities.SingleOrDefaultAsync(a => string.Equals(a.Name, author.AuthorName));
    }
}