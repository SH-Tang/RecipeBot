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
using System.Threading.Tasks;
using Common.Utils;
using Microsoft.EntityFrameworkCore;
using RecipeBot.Domain.Models;
using RecipeBot.Domain.Repositories;
using RecipeBot.Persistence.Creators;
using RecipeBot.Persistence.Entities;

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
            AuthorEntity authorEntity = await GetAuthorEntityAsync(model.Author);
            ICollection<RecipeTagEntity> tagLinks = await CreateRecipeTagEntities(model);

            RecipeEntity recipeEntity = RecipeEntityCreator.Create(model, authorEntity, tagLinks);
            context.RecipeEntities.Add(recipeEntity);

            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new Exception(ex.Message, ex); // TODO: Introduce custom exception
        }
    }

    private async Task<ICollection<RecipeTagEntity>> CreateRecipeTagEntities(RecipeModel model)
    {
        byte i = 0;
        var tagLinks = new List<RecipeTagEntity>();
        foreach (string tag in model.RecipeTags.Tags)
        {
            TagEntity tagEntity = await FindTagEntityAsync(tag) ?? new TagEntity
            {
                Tag = tag
            };

            tagLinks.Add(new RecipeTagEntity
            {
                Tag = tagEntity,
                Order = i++
            });
        }

        return tagLinks;
    }

    private async Task<AuthorEntity> GetAuthorEntityAsync(AuthorModel authorModel)
    {
        string authorName = authorModel.AuthorName;
        AuthorEntity? foundEntity = await context.AuthorEntities.SingleOrDefaultAsync(e => e.AuthorName == authorName);
        return foundEntity ?? AuthorEntityCreator.Create(authorModel);
    }

    private Task<TagEntity?> FindTagEntityAsync(string tag)
    {
        return context.TagEntities.SingleOrDefaultAsync(e => e.Tag == tag);
    }
}