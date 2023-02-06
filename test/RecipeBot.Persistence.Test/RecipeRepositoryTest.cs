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
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Models;
using RecipeBot.Domain.Repositories;
using RecipeBot.Domain.Repositories.Data;
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
        using(var context = new RecipeBotDbContext(new DbContextOptions<RecipeBotDbContext>()))
        {
            // Call
            var repository = new RecipeRepository(context);

            // Assert
            repository.Should().BeAssignableTo<IRecipeRepository>();
        }
    }

    [Theory]
    [InlineData(RecipeCategory.Dessert, PersistentRecipeCategory.Dessert)]
    [InlineData(RecipeCategory.Fish, PersistentRecipeCategory.Fish)]
    [InlineData(RecipeCategory.Meat, PersistentRecipeCategory.Meat)]
    [InlineData(RecipeCategory.Pastry, PersistentRecipeCategory.Pastry)]
    [InlineData(RecipeCategory.Snack, PersistentRecipeCategory.Snack)]
    [InlineData(RecipeCategory.Vegan, PersistentRecipeCategory.Vegan)]
    [InlineData(RecipeCategory.Vegetarian, PersistentRecipeCategory.Vegetarian)]
    [InlineData(RecipeCategory.Drinks, PersistentRecipeCategory.Drinks)]
    [InlineData(RecipeCategory.Other, PersistentRecipeCategory.Other)]
    public async Task Given_empty_database_when_saving_recipe_saves_recipe_with_expected_data(
        RecipeCategory category,
        PersistentRecipeCategory expectedCategory)
    {
        // Setup
        var fixture = new Fixture();
        RecipeModel recipeModel = testBuilder.SetCategory(category)
                                             .Build();

        Action<RecipeBotDbContext> assertPersistedDataAction =
            context =>
            {
                RecipeEntity recipeEntity = context.RecipeEntities
                                                   .AsNoTracking()
                                                   .Single();

                recipeEntity.RecipeCategory.Should().Be(expectedCategory);
            };

        // Call & Assert
        var test = new RecipeRepositoryPersistDataTest(recipeModel, assertPersistedDataAction);
        await test.ExecuteTest();
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
                RecipeEntity recipeEntity = context.RecipeEntities
                                                   .Include(e => e.Author)
                                                   .Include(e => e.RecipeFields)
                                                   .Include(e => e.Tags)
                                                   .AsNoTracking()
                                                   .Single();

                recipeEntity.RecipeTitle.Should().Be(recipeModel.Title);
                recipeEntity.Author.Should().NotBeNull().And.BeEquivalentTo(
                    recipeModel.Author,
                    options => options.Including(e => e.AuthorName)
                                      .Including(e => e.AuthorImageUrl));

                recipeEntity.RecipeFields.Should().BeEmpty();
                recipeEntity.Tags.Should().BeEmpty();
            };

        // Call & Assert
        var test = new RecipeRepositoryPersistDataTest(recipeModel, assertPersistedDataAction);
        await test.ExecuteTest();
    }

    [Fact]
    public async Task Given_empty_database_when_saving_recipe_with_all_data_saves_expected_data()
    {
        // Setup
        var fixture = new Fixture();
        string[] tags =
        {
            fixture.Create<string>(),
            fixture.Create<string>()
        };

        const int nrOfFields = 3;
        RecipeModel recipeModel = testBuilder.AddFields(nrOfFields)
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
                                                   .AsNoTracking()
                                                   .Single();

                recipeEntity.RecipeTitle.Should().Be(recipeModel.Title);
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
        var test = new RecipeRepositoryPersistDataTest(recipeModel, assertPersistedDataAction);
        await test.ExecuteTest();
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
                                                   .AsNoTracking()
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
        var test = new RecipeRepositoryPersistDataTest(recipeModel, assertPersistedDataAction, seedDatabaseAction);
        await test.ExecuteTest();
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
                                                   .AsNoTracking()
                                                   .Single();

                recipeEntity.Tags.OrderBy(t => t.Order).Should().Equal(tags, (s, e) => s.Tag.Tag == e);
                context.TagEntities.Should().HaveCount(tags.Length);
            };

        // Call & Assert
        var test = new RecipeRepositoryPersistDataTest(recipeModel, assertPersistedDataAction, seedDatabaseAction);
        await test.ExecuteTest();
    }

    [Fact]
    public async Task Given_empty_database_when_deleting_recipe_throws_exception()
    {
        // Setup
        using(var provider = new RecipeBotDBContextProvider())
        using(RecipeBotDbContext context = provider.CreateInMemoryContext())
        {
            await context.Database.EnsureCreatedAsync();

            var fixture = new Fixture();
            var repository = new RecipeRepository(context);

            var idToDelete = fixture.Create<long>();

            // Call
            Func<Task> call = () => repository.DeleteRecipeAsync(idToDelete);

            // Assert
            await call.Should().ThrowAsync<RepositoryDataDeleteException>()
                      .WithMessage($"No recipe matches with Id '{idToDelete}'.");
        }
    }

    [Fact]
    public async Task Given_seeded_database_when_deleting_recipe_only_deletes_affected_data_and_returns_result()
    {
        // Setup
        using(var provider = new RecipeBotDBContextProvider())
        using(RecipeBotDbContext context = provider.CreateInMemoryContext())
        {
            await context.Database.EnsureCreatedAsync();

            var fixture = new Fixture();
            var authorEntity = new AuthorEntity
            {
                AuthorName = fixture.Create<string>(),
                AuthorImageUrl = fixture.Create<string>()
            };

            IReadOnlyList<TagEntity> tagEntities = new[]
            {
                new TagEntity
                {
                    Tag = fixture.Create<string>()
                },
                new TagEntity
                {
                    Tag = fixture.Create<string>()
                },
                new TagEntity
                {
                    Tag = fixture.Create<string>()
                },
                new TagEntity
                {
                    Tag = fixture.Create<string>()
                }
            };

            var recipeToDelete = new RecipeEntity
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
                        Tag = tagEntities[0],
                        Order = fixture.Create<byte>()
                    },
                    new RecipeTagEntity
                    {
                        Tag = tagEntities[3],
                        Order = fixture.Create<byte>()
                    }
                }
            };

            var unaffectedRecipe = new RecipeEntity
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
                        Tag = tagEntities[1],
                        Order = fixture.Create<byte>()
                    },
                    new RecipeTagEntity
                    {
                        Tag = tagEntities[2],
                        Order = fixture.Create<byte>()
                    }
                }
            };

            await context.RecipeEntities.AddRangeAsync(recipeToDelete, unaffectedRecipe);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var repository = new RecipeRepository(context);

            // Call
            RecipeEntryData result = await repository.DeleteRecipeAsync(recipeToDelete.RecipeEntityId);
            context.ChangeTracker.Clear();

            // Assert
            result.Id.Should().Be(recipeToDelete.RecipeEntityId);
            result.Title.Should().Be(recipeToDelete.RecipeTitle);
            result.AuthorName.Should().Be(authorEntity.AuthorName);

            context.AuthorEntities.Should().BeEquivalentTo(new[]
            {
                authorEntity
            }, options => options.Excluding(s => s.Recipes));

            context.TagEntities.Should().BeEquivalentTo(tagEntities, options => options.Excluding(s => s.Recipes));
            context.RecipeFieldEntities.Should().BeEquivalentTo(unaffectedRecipe.RecipeFields, options => options.Excluding(s => s.Recipe));
            context.RecipeTagEntities.Should().BeEquivalentTo(unaffectedRecipe.Tags, options => options.Excluding(s => s.Recipe)
                                                                                                       .Excluding(s => s.Tag));

            RecipeEntity recipeEntity = await context.RecipeEntities
                                                     .Include(e => e.Author)
                                                     .Include(e => e.Tags)
                                                     .AsNoTracking()
                                                     .SingleAsync();
            recipeEntity.Should().BeEquivalentTo(unaffectedRecipe, options => options.Excluding(s => s.Author)
                                                                                     .Excluding(s => s.Tags)
                                                                                     .Excluding(s => s.RecipeFields));
            recipeEntity.Author.Should().BeEquivalentTo(unaffectedRecipe.Author, options => options.Excluding(s => s.Recipes));
            recipeEntity.Tags.Should().BeEquivalentTo(unaffectedRecipe.Tags, options => options.Excluding(s => s.Recipe)
                                                                                               .Excluding(s => s.Tag));

            recipeEntity.RecipeFields.Should().BeEquivalentTo(unaffectedRecipe.RecipeFields, options => options.Excluding(s => s.Recipe));
        }
    }

    [Fact]
    public async Task Given_empty_database_when_retrieving_recipe_throws_exception()
    {
        // Setup
        using(var provider = new RecipeBotDBContextProvider())
        using(RecipeBotDbContext context = provider.CreateInMemoryContext())
        {
            await context.Database.EnsureCreatedAsync();

            var fixture = new Fixture();
            var repository = new RecipeRepository(context);

            var idToRetrieve = fixture.Create<long>();

            // Call
            Func<Task> call = () => repository.GetRecipe(idToRetrieve);

            // Assert
            await call.Should().ThrowAsync<RepositoryDataLoadException>()
                      .WithMessage($"No recipe matches with Id '{idToRetrieve}'.");
        }
    }
    private class RecipeRepositoryPersistDataTest
    {
        private readonly Action<RecipeBotDbContext> assertPersistedDataAction;
        private readonly RecipeModel recipe;
        private readonly Action<RecipeBotDbContext>? seedDatabaseAction;

        public RecipeRepositoryPersistDataTest(RecipeModel recipe,
                                               Action<RecipeBotDbContext> assertPersistedDataAction,
                                               Action<RecipeBotDbContext>? seedDatabaseAction = null)
        {
            this.recipe = recipe;
            this.seedDatabaseAction = seedDatabaseAction;
            this.assertPersistedDataAction = assertPersistedDataAction;
        }

        public async Task ExecuteTest()
        {
            using(var provider = new RecipeBotDBContextProvider())
            using(RecipeBotDbContext context = provider.CreateInMemoryContext())
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
    }

    /// <summary>
    /// Test class for providing <see cref="RecipeBotDbContext"/>.
    /// </summary>
    private class RecipeBotDBContextProvider : IDisposable
    {
        private readonly SqliteConnection connection;

        public RecipeBotDBContextProvider()
        {
            connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            connection?.Dispose();
        }

        public RecipeBotDbContext CreateInMemoryContext()
        {
            DbContextOptions<RecipeBotDbContext> contextOptions =
                new DbContextOptionsBuilder<RecipeBotDbContext>().UseSqlite(connection)
                                                                 .Options;
            return new RecipeBotDbContext(contextOptions);
        }
    }
}