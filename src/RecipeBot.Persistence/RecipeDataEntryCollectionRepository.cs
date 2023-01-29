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
        IEnumerable<RecipeDatabaseEntry> recipeEntities = await context.RecipeEntities
                                                                       .Include(r => r.Author)
                                                                       .Select(e => new RecipeDatabaseEntry
                                                                       {
                                                                           Id = e.RecipeEntityId,
                                                                           Title = e.RecipeTitle,
                                                                           AuthorName = e.Author.AuthorName
                                                                       })
                                                                       .AsNoTracking()
                                                                       .ToArrayAsync();


        return CreateRecipeEntryDataCollection(recipeEntities);
    }

    public async Task<IReadOnlyList<RecipeEntryData>> LoadRecipeEntriesAsync(RecipeCategory category)
    {
        PersistentRecipeCategory persistentCategory = PersistentRecipeCategoryCreator.Create(category);

        IEnumerable<RecipeDatabaseEntry> recipeEntities = await context.RecipeEntities
                                                                       .Where(r => r.RecipeCategory == persistentCategory)
                                                                       .Include(r => r.Author)
                                                                       .Select(e => new RecipeDatabaseEntry
                                                                       {
                                                                           Id = e.RecipeEntityId,
                                                                           Title = e.RecipeTitle,
                                                                           AuthorName = e.Author.AuthorName
                                                                       })
                                                                       .AsNoTracking()
                                                                       .ToArrayAsync();

        return CreateRecipeEntryDataCollection(recipeEntities);
    }

    private static RecipeEntryData[] CreateRecipeEntryDataCollection(IEnumerable<RecipeDatabaseEntry> recipeDatabaseEntries)
    {
        return recipeDatabaseEntries.Select(r => new RecipeEntryData(r.Id, r.Title, r.AuthorName))
                                    .OrderBy(r => r.Id)
                                    .ToArray();
    }

    /// <summary>
    /// Class representing a simplified recipe entry in the database.
    /// </summary>
    private sealed class RecipeDatabaseEntry
    {
        /// <summary>
        /// Gets or sets the id of the recipe.
        /// </summary>
        public long Id { get; init; }

        /// <summary>
        /// Gets or sets the title of the recipe.
        /// </summary>
        public string Title { get; init; } = null!;

        /// <summary>
        /// Gets or sets the name of the author of the recipe.
        /// </summary>
        public string AuthorName { get; init; } = null!;
    }
}