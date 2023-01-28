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
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Models;
using RecipeBot.Domain.Repositories;
using RecipeBot.Domain.TestUtils;
using RecipeBot.Persistence.Entities;
using Xunit;

namespace RecipeBot.Persistence.Test;

public class RecipeRepositoryTest
{
    private readonly RecipeModelTestBuilder testBuilder;

    public RecipeRepositoryTest()
    {
        testBuilder = new RecipeModelTestBuilder();
    }

    [Fact]
    public void Repository_is_recipe_repository()
    {
        // Setup
        using (var context = new RecipeBotDbContext(new DbContextOptions<RecipeBotDbContext>()))
        {
            // Call
            var repository = new RecipeRepository(context);

            // Assert
            repository.Should().BeAssignableTo<IRecipeRepository>();
        }
    }

    [Fact]
    public async Task Given_empty_database_when_saving_basic_recipe_saves_expected_data()
    {
        // Setup
        var fixture = new Fixture();
        var category = fixture.Create<RecipeCategory>();
        RecipeModel recipeModel = testBuilder.SetCategory(category)
                                             .Build();

        Action<RecipeBotDbContext> assertPersistedDataAction =
            context =>
            {
                RecipeEntity? recipeEntity = context.RecipeEntities
                                                    .Include(e => e.Author)
                                                    .Include(e => e.RecipeFields)
                                                    .Include(e => e.Tags)
                                                    .Single();

                recipeEntity.RecipeTitle.Should().Be(recipeModel.Title);
                recipeEntity.RecipeCategory.Should().Be((PersistentRecipeCategory)category);

                recipeEntity.Author.Should().NotBeNull().And.BeEquivalentTo(
                    recipeModel.Author,
                    options => options.Including(e => e.AuthorName)
                                      .Including(e => e.AuthorImageUrl));

                recipeEntity.RecipeFields.Should().BeEmpty();
                recipeEntity.Tags.Should().BeEmpty();
            };

        // Call & Assert
        using (var test = new RecipeRepositoryPersistDataTest(recipeModel, assertPersistedDataAction))
        {
            await test.ExecuteTest();
        }
    }

    [Fact]
    public async Task Given_empty_database_when_saving_recipe_with_all_data_saves_expected_data()
    {
        // Setup
        var fixture = new Fixture();
        var category = fixture.Create<RecipeCategory>();

        string[] tags =
        {
            fixture.Create<string>(),
            fixture.Create<string>()
        };

        const int nrOfFields = 3;
        RecipeModel recipeModel = testBuilder.SetCategory(category)
                                             .AddFields(nrOfFields)
                                             .AddTags(tags)
                                             .Build();

        Action<RecipeBotDbContext> assertPersistedDataAction =
            context =>
            {
                RecipeEntity recipeEntity = context.RecipeEntities
                                                   .Include(e => e.Author)
                                                   .Include(e => e.RecipeFields)
                                                   .Include(e => e.Tags)
                                                   .ThenInclude(e => e.Tag)
                                                   .Single();

                recipeEntity.RecipeTitle.Should().Be(recipeModel.Title);
                recipeEntity.RecipeCategory.Should().Be((PersistentRecipeCategory)category);

                recipeEntity.Author.Should().NotBeNull().And.BeEquivalentTo(
                    recipeModel.Author,
                    options => options.Including(e => e.AuthorName)
                                      .Including(e => e.AuthorImageUrl));

                recipeEntity.RecipeFields.OrderBy(f => f.Order).Should().BeEquivalentTo(
                    recipeModel.RecipeFields,
                    options => options.WithStrictOrdering()
                                      .ExcludingMissingMembers()
                                      .WithMapping<RecipeFieldEntity>(e => e.FieldName, s => s.RecipeFieldName)
                                      .WithMapping<RecipeFieldEntity>(e => e.FieldData, s => s.RecipeFieldData));

                recipeEntity.Tags.OrderBy(t => t.Order).Should().Equal(tags, (s, e) => s.Tag.Tag == e);
            };

        // Call & Assert
        using (var test = new RecipeRepositoryPersistDataTest(recipeModel, assertPersistedDataAction))
        {
            await test.ExecuteTest();
        }
    }

    [Fact]
    public async Task Given_database_with_author_data_when_saving_recipe_with_author_then_recipe_links_to_author()
    {
        // Setup
        RecipeModel recipeModel = testBuilder.Build();

        Action<RecipeBotDbContext> seedDatabaseAction =
            context =>
            {
                context.AuthorEntities.Add(new AuthorEntity
                {
                    AuthorName = recipeModel.Author.AuthorName,
                    AuthorImageUrl = recipeModel.Author.AuthorImageUrl
                });
            };

        Action<RecipeBotDbContext> assertPersistedDataAction =
            context =>
            {
                RecipeEntity recipeEntity = context.RecipeEntities
                                                   .Include(e => e.Author)
                                                   .Single();
                AuthorEntity expectedAuthorEntity = context.AuthorEntities.Single();

                AuthorEntity authorEntity = recipeEntity.Author;
                authorEntity.Should().NotBeNull()
                            .And.BeSameAs(expectedAuthorEntity)
                            .And.BeEquivalentTo(
                                recipeModel.Author,
                                options => options.Including(e => e.AuthorName)
                                                  .Including(e => e.AuthorImageUrl));
            };

        // Call & Assert
        using (var test = new RecipeRepositoryPersistDataTest(recipeModel, assertPersistedDataAction, seedDatabaseAction))
        {
            await test.ExecuteTest();
        }
    }

    [Fact]
    public async Task Given_database_with_tag_data_when_saving_recipe_with_tag_then_recipe_links_to_tag()
    {
        // Setup
        var fixture = new Fixture();
        string[] tags =
        {
            fixture.Create<string>(),
            fixture.Create<string>()
        };

        RecipeModel recipeModel = testBuilder.AddTags(tags)
                                             .Build();

        Action<RecipeBotDbContext> seedDatabaseAction =
            context =>
            {
                context.TagEntities.AddRange(tags.Reverse().Select(t => new TagEntity
                {
                    Tag = t
                }));
            };

        Action<RecipeBotDbContext> assertPersistedDataAction =
            context =>
            {
                RecipeEntity recipeEntity = context.RecipeEntities
                                                   .Include(e => e.Tags)
                                                   .ThenInclude(e => e.Tag)
                                                   .Single();

                recipeEntity.Tags.OrderBy(t => t.Order).Should().Equal(tags, (s, e) => s.Tag.Tag == e);
                context.TagEntities.Should().HaveCount(tags.Length);
            };

        // Call & Assert
        using (var test = new RecipeRepositoryPersistDataTest(recipeModel, assertPersistedDataAction, seedDatabaseAction))
        {
            await test.ExecuteTest();
        }
    }

    private class RecipeRepositoryPersistDataTest : IDisposable
    {
        private readonly RecipeModel recipe;
        private readonly Action<RecipeBotDbContext>? seedDatabaseAction;
        private readonly Action<RecipeBotDbContext> assertPersistedDataAction;
        private readonly SqliteConnection connection;

        public RecipeRepositoryPersistDataTest(RecipeModel recipe,
                                               Action<RecipeBotDbContext> assertPersistedDataAction,
                                               Action<RecipeBotDbContext>? seedDatabaseAction = null)
        {
            this.recipe = recipe;
            this.seedDatabaseAction = seedDatabaseAction;
            this.assertPersistedDataAction = assertPersistedDataAction;

            connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
        }

        public async Task ExecuteTest()
        {
            using (RecipeBotDbContext context = CreateInMemoryContext())
            {
                await context.Database.EnsureCreatedAsync();

                if (seedDatabaseAction != null)
                {
                    seedDatabaseAction(context);
                    await context.SaveChangesAsync();

                    context.ChangeTracker.Clear();
                }

                var repository = new RecipeRepository(context);
                await repository.SaveRecipeAsync(recipe);
                context.ChangeTracker.Clear();

                assertPersistedDataAction(context);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            connection.Dispose();
        }

        private RecipeBotDbContext CreateInMemoryContext()
        {
            DbContextOptions<RecipeBotDbContext> contextOptions =
                new DbContextOptionsBuilder<RecipeBotDbContext>().UseSqlite(connection)
                                                                 .Options;
            return new RecipeBotDbContext(contextOptions);
        }
    }
}