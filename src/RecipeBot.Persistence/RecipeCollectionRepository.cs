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
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Repositories;
using RecipeBot.Domain.Repositories.Data;
using RecipeBot.Persistence.Creators;
using RecipeBot.Persistence.Entities;
using RecipeBot.Persistence.Properties;

namespace RecipeBot.Persistence;

/// <summary>
/// An EF Core implementation of the <see cref="IRecipeCollectionRepository"/>.
/// </summary>
public class RecipeCollectionRepository : IRecipeCollectionRepository
{
    private readonly RecipeBotDbContext context;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeCollectionRepository"/>.
    /// </summary>
    /// <param name="context">The <see cref="RecipeBotDbContext"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <c>null</c>.</exception>
    public RecipeCollectionRepository(RecipeBotDbContext context)
    {
        context.IsNotNull(nameof(context));

        this.context = context;
    }

    public async Task<IReadOnlyList<RecipeEntryRepositoryData>> LoadRecipeEntriesAsync()
    {
        IEnumerable<RecipeDatabaseEntry> recipeEntities = await context.RecipeEntities
                                                                       .Include(r => r.Author)
                                                                       .Select(e => CreateRecipeDatabaseEntry(e))
                                                                       .AsNoTracking()
                                                                       .ToArrayAsync();

        return CreateRecipeEntryDataCollection(recipeEntities);
    }

    public async Task<IReadOnlyList<RecipeEntryRepositoryData>> LoadRecipeEntriesByCategoryAsync(RecipeCategory category)
    {
        PersistentRecipeCategory persistentCategory = PersistentRecipeCategoryCreator.Create(category);

        IEnumerable<RecipeDatabaseEntry> recipeEntities = await context.RecipeEntities
                                                                       .Where(r => r.RecipeCategory == persistentCategory)
                                                                       .Include(r => r.Author)
                                                                       .Select(e => CreateRecipeDatabaseEntry(e))
                                                                       .AsNoTracking()
                                                                       .ToArrayAsync();

        return CreateRecipeEntryDataCollection(recipeEntities);
    }

    public async Task<IReadOnlyList<RecipeEntryRepositoryData>> LoadRecipeEntriesByTagAsync(string tag)
    {
        IEnumerable<RecipeDatabaseEntry> recipeDatabaseEntries = await context.RecipeTagEntities
                                                                              .Include(te => te.Tag)
                                                                              .Where(te => te.Tag.Tag == tag)
                                                                              .Include(te => te.Recipe)
                                                                              .ThenInclude(r => r.Author)
                                                                              .Select(e => CreateRecipeDatabaseEntry(e))
                                                                              .AsNoTracking()
                                                                              .ToArrayAsync();
        return CreateRecipeEntryDataCollection(recipeDatabaseEntries);
    }

    public async Task<IReadOnlyList<RecipeEntryRepositoryData>> LoadRecipeEntriesByTagIdAsync(long tagEntityId)
    {
        IEnumerable<RecipeDatabaseEntry> recipeDatabaseEntries = await context.RecipeTagEntities
                                                                              .Include(te => te.Tag)
                                                                              .Where(te => te.Tag.TagEntityId == tagEntityId)
                                                                              .Include(te => te.Recipe)
                                                                              .ThenInclude(r => r.Author)
                                                                              .Select(e => CreateRecipeDatabaseEntry(e))
                                                                              .AsNoTracking()
                                                                              .ToArrayAsync();
        return CreateRecipeEntryDataCollection(recipeDatabaseEntries);
    }

    public async Task<IReadOnlyList<RecipeEntryRepositoryData>> LoadRecipeEntriesByAuthorIdAsync(ulong authorId)
    {
        IEnumerable<RecipeDatabaseEntry> recipeEntries = await context.AuthorEntities
                                                                      .Where(a => a.AuthorId == authorId.ToString())
                                                                      .Include(e => e.Recipes)
                                                                      .ThenInclude(e => e.Author)
                                                                      .SelectMany(e => e.Recipes)
                                                                      .Select(e => CreateRecipeDatabaseEntry(e))
                                                                      .AsNoTracking()
                                                                      .ToArrayAsync();

        return CreateRecipeEntryDataCollection(recipeEntries);
    }

    /// <summary>
    /// Creates a database entry based on the input arguments.
    /// </summary>
    /// <param name="entity">The entity to create the entry with.</param>
    /// <returns>A <see cref="RecipeDatabaseEntry"/>.</returns>
    /// <exception cref="RepositoryDataLoadException">Thrown when the entry could not be successfully created.</exception>
    private static RecipeDatabaseEntry CreateRecipeDatabaseEntry(RecipeTagEntity entity)
    {
        string authorId = entity.Recipe.Author.AuthorId;
        try
        {
            return new RecipeDatabaseEntry
            {
                Id = entity.RecipeEntityId,
                Title = entity.Recipe.RecipeTitle,
                AuthorId = ulong.Parse(authorId)
            };
        }
        catch (Exception e) when (e is OverflowException || e is FormatException)
        {
            throw new RepositoryDataLoadException(string.Format(Resources.Recipe_entries_unsuccessfully_loaded_due_to_invalid_AuthorId_0, authorId), e);
        }
    }

    /// <summary>
    /// Creates a database entry based on the input arguments.
    /// </summary>
    /// <param name="entity">The entity to create the entry with.</param>
    /// <returns>A <see cref="RecipeDatabaseEntry"/>.</returns>
    /// <exception cref="RepositoryDataLoadException">Thrown when the entry could not be successfully created.</exception>
    private static RecipeDatabaseEntry CreateRecipeDatabaseEntry(RecipeEntity entity)
    {
        string authorId = entity.Author.AuthorId;
        try
        {
            return new RecipeDatabaseEntry
            {
                Id = entity.RecipeEntityId,
                Title = entity.RecipeTitle,
                AuthorId = ulong.Parse(entity.Author.AuthorId)
            };
        }
        catch (Exception e) when (e is OverflowException || e is FormatException)
        {
            throw new RepositoryDataLoadException(string.Format(Resources.Recipe_entries_unsuccessfully_loaded_due_to_invalid_AuthorId_0, authorId), e);
        }
    }

    private static RecipeEntryRepositoryData[] CreateRecipeEntryDataCollection(IEnumerable<RecipeDatabaseEntry> recipeDatabaseEntries)
    {
        return recipeDatabaseEntries.Select(r => new RecipeEntryRepositoryData(r.Id, r.Title, r.AuthorId))
                                    .OrderBy(r => r.EntityId)
                                    .ToArray();
    }

    /// <summary>
    /// Class representing a simplified recipe entry in the database.
    /// </summary>
    private sealed record RecipeDatabaseEntry
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
        /// Gets or sets the id of the author of the recipe.
        /// </summary>
        public ulong AuthorId { get; init; }
    }
}