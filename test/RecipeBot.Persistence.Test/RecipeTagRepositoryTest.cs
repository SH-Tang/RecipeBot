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
using RecipeBot.Domain.Repositories.Data;
using RecipeBot.Persistence.Entities;
using Xunit;

namespace RecipeBot.Persistence.Test;

public class RecipeTagRepositoryTest : IDisposable
{
    private readonly SqliteConnection connection;

    public RecipeTagRepositoryTest()
    {
        connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        connection.Dispose();
    }

    [Fact]
    public async Task Given_empty_database_when_loading_tags_returns_empty_collection()
    {
        // Setup
        using(RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();

            var repository = new RecipeEntryRepository(context);

            // Call
            IReadOnlyList<RecipeTagEntryData> entries = await repository.LoadRecipeTagEntriesAsync();

            // Assert
            entries.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task Given_seeded_database_when_loading_tags_returns_expected_tag_entries()
    {
        using(RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();

            var fixture = new Fixture();
            IReadOnlyList<TagEntity> tagEntities = new[]
            {
                new TagEntity
                {
                    TagEntityId = 2,
                    Tag = fixture.Create<string>()
                },
                new TagEntity
                {
                    TagEntityId = 1,
                    Tag = fixture.Create<string>()
                }
            };

            context.TagEntities.AddRange(tagEntities);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var repository = new RecipeEntryRepository(context);

            // Call
            IReadOnlyList<RecipeTagEntryData> entries = await repository.LoadRecipeTagEntriesAsync();

            // Assert
            entries.Should().BeEquivalentTo(tagEntities, options => options.ExcludingMissingMembers()
                                                                           .WithStrictOrderingFor(e => e.TagEntityId)
                                                                           .WithMapping<TagEntity, RecipeTagEntryData>(s => s.TagEntityId, e => e.Id)
                                                                           .WithMapping<TagEntity, RecipeTagEntryData>(s => s.Tag, e => e.Tag));
        }
    }

    private RecipeBotDbContext CreateContext()
    {
        DbContextOptions<RecipeBotDbContext> contextOptions =
            new DbContextOptionsBuilder<RecipeBotDbContext>().UseSqlite(connection)
                                                             .Options;
        return new RecipeBotDbContext(contextOptions);
    }
}