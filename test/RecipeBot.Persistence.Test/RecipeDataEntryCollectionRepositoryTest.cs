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
using AutoFixture;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Repositories.Data;
using RecipeBot.Persistence.Entities;
using Xunit;

namespace RecipeBot.Persistence.Test;

public class RecipeDataEntryCollectionRepositoryTest : IDisposable
{
    private readonly SqliteConnection connection;

    public RecipeDataEntryCollectionRepositoryTest()
    {
        connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
    }

    [Fact]
    public async Task Given_empty_database_when_loading_entries_returns_empty_collection()
    {
        // Setup
        using (RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();

            var repository = new RecipeDataEntryCollectionRepository(context);

            // Call
            IReadOnlyList<RecipeEntryData> entries = await repository.LoadRecipeEntriesAsync();

            // Assert
            entries.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task Given_seeded_database_when_loading_entries_returns_all_entry_data_on_sorted_id()
    {
        // Setup
        var fixture = new Fixture();

        var authorOne = new AuthorEntity
        {
            AuthorName = fixture.Create<string>(),
            AuthorImageUrl = fixture.Create<string>()
        };
        var authorTwo = new AuthorEntity
        {
            AuthorName = fixture.Create<string>(),
            AuthorImageUrl = fixture.Create<string>()
        };

        var recipeOne = new RecipeEntity
        {
            RecipeEntityId = fixture.Create<int>(),
            Author = authorOne,
            RecipeTitle = fixture.Create<string>()
        };
        var recipeTwo = new RecipeEntity
        {
            RecipeEntityId = fixture.Create<int>(),
            Author = authorTwo,
            RecipeTitle = fixture.Create<string>()
        };
        var recipeThree = new RecipeEntity
        {
            RecipeEntityId = fixture.Create<int>(),
            Author = authorOne,
            RecipeTitle = fixture.Create<string>()
        };

        RecipeEntity[] recipes =
        {
            recipeOne,
            recipeTwo,
            recipeThree
        };

        using (RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();
            await context.RecipeEntities.AddRangeAsync(recipes);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var repository = new RecipeDataEntryCollectionRepository(context);

            // Call
            IReadOnlyList<RecipeEntryData> entries = await repository.LoadRecipeEntriesAsync();

            // Assert
            entries.Should().BeInAscendingOrder(s => s.Id).And.BeEquivalentTo(
                recipes,
                options => options.ExcludingMissingMembers()
                                  .WithMapping<RecipeEntryData>(e => e.RecipeEntityId, s => s.Id)
                                  .WithMapping<RecipeEntryData>(e => e.RecipeTitle, s => s.Title)
                                  .WithMapping<AuthorEntity, RecipeEntryData>(e => e.AuthorName, s => s.AuthorName));
        }
    }

    [Fact]
    public async Task Given_empty_database_when_loading_entries_with_category_returns_empty_collection()
    {
        // Setup
        var fixture = new Fixture();
        using (RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();

            var repository = new RecipeDataEntryCollectionRepository(context);

            // Call
            IReadOnlyList<RecipeEntryData> entries = await repository.LoadRecipeEntriesAsync(fixture.Create<RecipeCategory>());

            // Assert
            entries.Should().BeEmpty();
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        connection.Dispose();
    }

    private RecipeBotDbContext CreateContext()
    {
        DbContextOptions<RecipeBotDbContext> contextOptions =
            new DbContextOptionsBuilder<RecipeBotDbContext>().UseSqlite(connection)
                                                             .Options;
        return new RecipeBotDbContext(contextOptions);
    }
}