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
using Microsoft.EntityFrameworkCore;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Models;
using RecipeBot.Domain.Repositories;
using RecipeBot.Domain.Repositories.DTO;
using RecipeBot.Persistence.Entities;

namespace RecipeBot.Persistence.Repositories;

/// <summary>
/// An Entity Framework Core implementation of <see cref="IRecipeRepository"/>.
/// </summary>
public class RecipeRepository : IRecipeRepository
{
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
            Category = (PersistentRecipeCategory) recipe.RecipeCategory, // Mappings are currently one to one
            Author = authorEntity
        };

        context.RecipeEntities.Add(recipeEntity);
        await context.SaveChangesAsync(); // Might throw exceptions, catch the lower level exceptions
    }

    public async Task<RecipeDto?> DeleteRecipeAsync(int id)
    {
        RecipeEntity? entity = await context.RecipeEntities
                                            .SingleOrDefaultAsync(r => r.RecipeEntityId == id);
        if (entity == null)
        {
            return null;
        }

        context.RecipeEntities.Remove(entity);
        await context.SaveChangesAsync();

        return CreateRecipeDto(entity);
    }

    public async Task<RecipeDto?> GetRecipeByIdAsync(int id)
    {
        RecipeEntity? entity = await context.RecipeEntities
                                            .AsNoTracking()
                                            .Include(e => e.Author)
                                            .SingleOrDefaultAsync(e => e.RecipeEntityId == id);
        return entity == null ? null : CreateRecipeDto(entity);
    }

    public async Task<IEnumerable<RecipeDto>> GetAllRecipes()
    {
        IEnumerable<RecipeEntity> entities = await context.RecipeEntities
                                                          .AsNoTracking()
                                                          .Include(e => e.Author)
                                                          .ToArrayAsync();

        return entities.Select(CreateRecipeDto).ToArray();
    }

    private static RecipeDto CreateRecipeDto(RecipeEntity entity)
    {
        AuthorDto? author = null;
        if (entity.Author != null) // Only NULL when it's not being eagerly loaded in the DB context
        {
            author = new AuthorDto
            {
                Name = entity.Author.Name
            };
        }

        return new RecipeDto
        {
            Id = entity.RecipeEntityId,
            Category = (RecipeCategory) entity.Category,
            Author = author,
            Title = entity.Title
        };
    }

    private Task<AuthorEntity?> FindAuthorAsync(AuthorModel author)
    {
        return context.AuthorEntities.SingleOrDefaultAsync(a => string.Equals(a.Name, author.AuthorName));
    }
}