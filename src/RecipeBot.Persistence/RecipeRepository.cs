﻿// Copyright (C) 2022 Dennis Tang. All rights reserved.
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
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Models;
using RecipeBot.Domain.Repositories;
using RecipeBot.Domain.Repositories.Data;
using RecipeBot.Persistence.Creators;
using RecipeBot.Persistence.Entities;
using RecipeBot.Persistence.Properties;
using RecipeBot.Persistence.Readers;

namespace RecipeBot.Persistence;

/// <summary>
/// An EF Core implementation of the <see cref="IRecipeRepository"/>.
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

    public async Task SaveRecipeAsync(RecipeModel model)
    {
        model.IsNotNull(nameof(model));

        try
        {
            AuthorEntity authorEntity = await GetAuthorEntityAsync(model.AuthorId);
            ICollection<RecipeTagEntity> tagLinks = await CreateRecipeTagEntities(model);

            RecipeEntity recipeEntity = RecipeEntityCreator.Create(model, authorEntity, tagLinks);
            context.RecipeEntities.Add(recipeEntity);

            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryDataSaveException(ex.Message, ex);
        }
    }

    public async Task<RecipeEntryData> DeleteRecipeAsync(long id)
    {
        try
        {
            RecipeEntity? entityToDelete = await context.RecipeEntities
                                                        .Include(e => e.RecipeFields)
                                                        .Include(e => e.Tags)
                                                        .Include(e => e.Author)
                                                        .SingleOrDefaultAsync(e => e.RecipeEntityId == id);
            if (entityToDelete == null)
            {
                throw new RepositoryDataDeleteException(string.Format(Resources.RecipeRepository_No_recipe_matches_with_Id_0, id));
            }

            context.RecipeEntities.Remove(entityToDelete);
            await context.SaveChangesAsync();

            // TODO Wrap the parse in an outer exception
            return new RecipeEntryData(entityToDelete.RecipeEntityId, entityToDelete.RecipeTitle, ulong.Parse(entityToDelete.Author.AuthorId));
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryDataSaveException(ex.Message, ex);
        }
    }

    public async Task<RecipeData> GetRecipeAsync(long id)
    {
        RecipeEntity? entityToRetrieve = await context.RecipeEntities
                                                      .Include(e => e.RecipeFields)
                                                      .Include(e => e.Author)
                                                      .Include(e => e.Tags)
                                                      .ThenInclude(e => e.Tag)
                                                      .AsNoTracking()
                                                      .SingleOrDefaultAsync(e => e.RecipeEntityId == id);
        if (entityToRetrieve == null)
        {
            throw new RepositoryDataLoadException(string.Format(Resources.RecipeRepository_No_recipe_matches_with_Id_0, id));
        }

        return RecipeDataReader.Read(entityToRetrieve);
    }

    private async Task<ICollection<RecipeTagEntity>> CreateRecipeTagEntities(RecipeModel model)
    {
        byte i = 0;
        Task<RecipeTagEntity>[] tasks = model.RecipeTags.Tags.Select(t => CreateRecipeTagEntity(t, i++)).ToArray();
        await Task.WhenAll(tasks);

        return tasks.Select(task => task.Result).ToArray();
    }

    private async Task<RecipeTagEntity> CreateRecipeTagEntity(string tag, byte i)
    {
        TagEntity tagEntity = await FindTagEntityAsync(tag) ?? new TagEntity
        {
            Tag = tag
        };

        return new RecipeTagEntity
        {
            Tag = tagEntity, 
            Order = i
        };
    }

    private async Task<AuthorEntity> GetAuthorEntityAsync(ulong authorId)
    {
        AuthorEntity? foundEntity = await context.AuthorEntities.SingleOrDefaultAsync(e => e.AuthorId == authorId.ToString());
        return foundEntity ?? new AuthorEntity
        {
            AuthorId = authorId.ToString()
        };
    }

    private Task<TagEntity?> FindTagEntityAsync(string tag)
    {
        return context.TagEntities.SingleOrDefaultAsync(e => e.Tag == tag);
    }
}