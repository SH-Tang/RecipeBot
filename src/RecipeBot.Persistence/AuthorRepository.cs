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
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Repositories;
using RecipeBot.Persistence.Entities;
using RecipeBot.Persistence.Properties;

namespace RecipeBot.Persistence;

/// <summary>
/// An EF Core implementation of the <see cref="IAuthorRepository"/>.
/// </summary>
public class AuthorRepository : IAuthorRepository
{
    private readonly RecipeBotDbContext context;

    /// <summary>
    /// Creates a new instance of <see cref="AuthorRepository"/>.
    /// </summary>
    /// <param name="context">The <see cref="RecipeBotDbContext"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <c>null</c>.</exception>
    public AuthorRepository(RecipeBotDbContext context)
    {
        context.IsNotNull(nameof(context));

        this.context = context;
    }

    public async Task DeleteEntityAsync(ulong authorId)
    {
        AuthorEntity? entityToDelete = await context.AuthorEntities.SingleOrDefaultAsync(e => e.AuthorId == authorId.ToString());
        if (entityToDelete == null)
        {
            throw new RepositoryDataDeleteException(Resources.AuthorRepository_Delete_AuthorEntity_No_matching_author_found);
        }

        try
        {
            context.AuthorEntities.Remove(entityToDelete);
            await context.SaveChangesAsync();
            await Task.CompletedTask;
        }
        catch (DbUpdateException ex)
        {
            throw new RepositoryDataDeleteException(ex.Message, ex);
        }
    }
}