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
using AutoFixture;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Repositories.Data;
using RecipeBot.Persistence.Entities;
using Xunit;

namespace RecipeBot.Persistence.Test;

public class RecipeTagEntryRepositoryTest : IDisposable
{
    private readonly SqliteConnection connection;

    public RecipeTagEntryRepositoryTest()
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

            var repository = new RecipeTagEntryRepository(context);

            // Call
            IReadOnlyList<RecipeTagEntryData> entries = await repository.LoadRecipeTagEntriesAsync();

            // Assert
            entries.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task Given_seeded_database_when_loading_tags_returns_expected_tag_entries()
    {
        // Setup
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

            var repository = new RecipeTagEntryRepository(context);

            // Call
            IReadOnlyList<RecipeTagEntryData> entries = await repository.LoadRecipeTagEntriesAsync();

            // Assert
            entries.Should().BeEquivalentTo(tagEntities, options => options.ExcludingMissingMembers()
                                                                           .WithStrictOrderingFor(e => e.TagEntityId)
                                                                           .WithMapping<TagEntity, RecipeTagEntryData>(s => s.TagEntityId, e => e.Id)
                                                                           .WithMapping<TagEntity, RecipeTagEntryData>(s => s.Tag, e => e.Tag));
        }
    }

    [Fact]
    public async Task Given_empty_database_when_deleting_tags_throws_exception()
    {
        // Setup
        using(RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();

            var repository = new RecipeTagEntryRepository(context);

            var fixture = new Fixture();
            var idToDelete = fixture.Create<long>();

            // Call
            Func<Task> call = () => repository.DeleteTagAsync(idToDelete);

            // Assert
            await call.Should().ThrowAsync<RepositoryDataDeleteException>()
                      .WithMessage($"No tag matches with id '{idToDelete}'.");
        }
    }

    [Fact]
    public async Task Given_seeded_database_when_deleting_tag_only_deletes_affected_data_and_returns_result()
    {
        // Setup
        using(RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();

            var fixture = new Fixture();
            var authorEntity = new AuthorEntity
            {
                AuthorName = fixture.Create<string>(),
                AuthorImageUrl = fixture.Create<string>()
            };

            var tagToDelete = new TagEntity
            {
                TagEntityId = fixture.Create<long>(),
                Tag = fixture.Create<string>()
            };
            var unaffectedTag = new TagEntity
            {
                Tag = fixture.Create<string>()
            };

            var recipe = new RecipeEntity
            {
                RecipeEntityId = fixture.Create<long>(),
                RecipeTitle = fixture.Create<string>(),
                Author = authorEntity,
                RecipeCategory = fixture.Create<PersistentRecipeCategory>(),
                RecipeFields = new[]
                {
                    new RecipeFieldEntity
                    {
                        RecipeFieldData = fixture.Create<string>(),
                        RecipeFieldName = fixture.Create<string>(),
                        Order = fixture.Create<byte>()
                    },
                    new RecipeFieldEntity
                    {
                        RecipeFieldData = fixture.Create<string>(),
                        RecipeFieldName = fixture.Create<string>(),
                        Order = fixture.Create<byte>()
                    }
                },
                Tags = new[]
                {
                    new RecipeTagEntity
                    {
                        Tag = tagToDelete,
                        Order = fixture.Create<byte>()
                    },
                    new RecipeTagEntity
                    {
                        Tag = unaffectedTag,
                        Order = fixture.Create<byte>()
                    }
                }
            };

            context.RecipeEntities.Add(recipe);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var repository = new RecipeTagEntryRepository(context);

            // Call
            RecipeTagEntryData result = await repository.DeleteTagAsync(tagToDelete.TagEntityId);
            context.ChangeTracker.Clear();

            // Assert
            result.Id.Should().Be(tagToDelete.TagEntityId);
            result.Tag.Should().Be(tagToDelete.Tag);

            context.AuthorEntities.Should().BeEquivalentTo(new[]
            {
                authorEntity
            }, options => options.Excluding(s => s.Recipes));

            context.TagEntities.Should().BeEquivalentTo(new[]
            {
                unaffectedTag
            }, options => options.Excluding(s => s.Recipes));


            context.RecipeFieldEntities.Should().BeEquivalentTo(recipe.RecipeFields, options => options.Excluding(s => s.Recipe));
            context.RecipeTagEntities.Should().BeEquivalentTo(new[]
            {
                recipe.Tags.ElementAt(1)
            }, options => options.Excluding(s => s.Recipe)
                                 .Excluding(s => s.Tag));

            RecipeEntity recipeEntity = await context.RecipeEntities
                                                     .Include(e => e.Author)
                                                     .Include(e => e.RecipeFields)
                                                     .Include(e => e.Tags)
                                                     .AsNoTracking()
                                                     .SingleAsync();
            recipeEntity.Should().BeEquivalentTo(recipe, options => options.Excluding(s => s.Author)
                                                                           .Excluding(s => s.Tags)
                                                                           .Excluding(s => s.RecipeFields));
            recipeEntity.Author.Should().BeEquivalentTo(recipe.Author, options => options.Excluding(s => s.Recipes));
            recipeEntity.Tags.Should().BeEquivalentTo(new[]
            {
                recipe.Tags.ElementAt(1)
            }, options => options.Excluding(s => s.Recipe)
                                 .Excluding(s => s.Tag));

            recipeEntity.RecipeFields.Should().BeEquivalentTo(recipe.RecipeFields, options => options.Excluding(s => s.Recipe));
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