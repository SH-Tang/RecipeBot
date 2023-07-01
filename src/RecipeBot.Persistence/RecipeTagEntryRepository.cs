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
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Repositories;
using RecipeBot.Domain.Repositories.Data;
using RecipeBot.Persistence.Entities;
using RecipeBot.Persistence.Properties;

namespace RecipeBot.Persistence;

/// <summary>
/// An EF implementation of <see cref="IRecipeTagEntryDataRepository"/>.
/// </summary>
public class RecipeTagEntryRepository : IRecipeTagEntryDataRepository
{
    private readonly RecipeBotDbContext context;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeTagEntryRepository"/>.
    /// </summary>
    /// <param name="context">The <see cref="RecipeBotDbContext"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <c>null</c>.</exception>
    public RecipeTagEntryRepository(RecipeBotDbContext context)
    {
        context.IsNotNull(nameof(context));
        this.context = context;
    }

    public async Task<IReadOnlyList<RecipeTagRepositoryEntityData>> LoadRecipeTagEntriesAsync()
    {
        TagEntity[] tagEntities = await context.TagEntities.AsNoTracking().ToArrayAsync();

        return tagEntities.OrderBy(e => e.TagEntityId)
                          .Select(e => new RecipeTagRepositoryEntityData(e.TagEntityId, e.Tag))
                          .ToArray();
    }

    public async Task<RecipeTagRepositoryEntityData> DeleteTagAsync(long entityId)
    {
        try
        {
            TagEntity? tagToDelete = await context.TagEntities.SingleOrDefaultAsync(e => e.TagEntityId == entityId);
            if (tagToDelete == null)
            {
                throw new RepositoryDataDeleteException(string.Format(Resources.RecipeTagEntryRepository_DeleteTagAsync_No_tag_matches_with_EntityId_0_, entityId));
            }

            context.TagEntities.Remove(tagToDelete);
            await context.SaveChangesAsync();

            return new RecipeTagRepositoryEntityData(tagToDelete.TagEntityId, tagToDelete.Tag);
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryDataDeleteException(ex.Message, ex);
        }
    }
}