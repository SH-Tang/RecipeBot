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
using RecipeBot.Domain.Repositories;
using RecipeBot.Domain.Repositories.Data;
using RecipeBot.Persistence.Creators;
using RecipeBot.Persistence.Entities;

namespace RecipeBot.Persistence;

/// <summary>
/// An EF Core implementation of the <see cref="IRecipeDataEntryCollectionRepository"/>.
/// </summary>
public class RecipeDataEntryCollectionRepository : IRecipeDataEntryCollectionRepository
{
    private readonly RecipeBotDbContext context;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeDataEntryCollectionRepository"/>.
    /// </summary>
    /// <param name="context">The <see cref="RecipeBotDbContext"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <c>null</c>.</exception>
    public RecipeDataEntryCollectionRepository(RecipeBotDbContext context)
    {
        context.IsNotNull(nameof(context));

        this.context = context;
    }

    public async Task<IReadOnlyList<RecipeEntryData>> LoadRecipeEntriesAsync()
    {
        IEnumerable<RecipeEntity> recipeEntities = await context.RecipeEntities
                                                                .Include(r => r.Author)
                                                                .AsNoTracking()
                                                                .ToArrayAsync();
        return CreateRecipeEntryDataCollection(recipeEntities);
    }

    public async Task<IReadOnlyList<RecipeEntryData>> LoadRecipeEntriesAsync(RecipeCategory category)
    {
        PersistentRecipeCategory persistentCategory = PersistentRecipeCategoryCreator.Create(category);

        IEnumerable<RecipeEntity> recipeEntities = await context.RecipeEntities
                                                                .Where(r => r.RecipeCategory == persistentCategory)
                                                                .Include(r => r.Author)
                                                                .AsNoTracking()
                                                                .ToArrayAsync();

        return CreateRecipeEntryDataCollection(recipeEntities);
    }

    private static RecipeEntryData[] CreateRecipeEntryDataCollection(IEnumerable<RecipeEntity> recipeEntities)
    {
        return recipeEntities.Select(r => new RecipeEntryData(r.RecipeEntityId, r.RecipeTitle, r.Author.AuthorName))
            .OrderBy(r => r.Id)
            .ToArray();
    }
}