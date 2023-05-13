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
using RecipeBot.Domain.Repositories.Data;
using RecipeBot.Persistence.Creators;
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
        using(RecipeBotDbContext context = CreateContext())
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
            AuthorId = fixture.Create<ulong>().ToString()
        };
        var authorTwo = new AuthorEntity
        {
            AuthorId = fixture.Create<ulong>().ToString()
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

        using(RecipeBotDbContext context = CreateContext())
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
                                  .WithAutoConversion()
                                  .WithMapping<RecipeEntryData>(e => e.RecipeEntityId, s => s.Id)
                                  .WithMapping<RecipeEntryData>(e => e.RecipeTitle, s => s.Title)
                                  .WithMapping<AuthorEntity, RecipeEntryData>(e => e.AuthorId, s => s.AuthorId));
        }
    }

    [Theory]
    [MemberData(nameof(GetInvalidAuthorId))]
    public async Task Given_database_with_invalid_author_when_loading_entries_throws_exception(string invalidAuthorId)
    {
        // Setup
        var invalidAuthorEntity = new AuthorEntity
        {
            AuthorId = invalidAuthorId
        };

        var fixture = new Fixture();
        var recipe = new RecipeEntity
        {
            RecipeEntityId = fixture.Create<int>(),
            Author = invalidAuthorEntity,
            RecipeTitle = fixture.Create<string>()
        };

        using (RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();
            await context.RecipeEntities.AddAsync(recipe);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var repository = new RecipeDataEntryCollectionRepository(context);

            // Call
            Func<Task> call = () => repository.LoadRecipeEntriesAsync();

            // Assert
            await call.Should().ThrowAsync<RepositoryDataLoadException>()
                      .WithMessage($"Recipe entries could not be loaded due to invalid AuthorId '{invalidAuthorId}'.");
        }
    }

    [Fact]
    public async Task Given_empty_database_when_loading_entries_by_category_returns_empty_collection()
    {
        // Setup
        var fixture = new Fixture();
        using(RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();

            var repository = new RecipeDataEntryCollectionRepository(context);

            // Call
            IReadOnlyList<RecipeEntryData> entries = await repository.LoadRecipeEntriesByCategoryAsync(fixture.Create<RecipeCategory>());

            // Assert
            entries.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task Given_seeded_database_when_loading_entries_with_category_and_no_match_found_returns_empty_collection()
    {
        // Setup
        var fixture = new Fixture();

        var author = new AuthorEntity
        {
            AuthorId = fixture.Create<string>()
        };

        var recipeOne = new RecipeEntity
        {
            RecipeEntityId = fixture.Create<int>(),
            Author = author,
            RecipeCategory = PersistentRecipeCategory.Other,
            RecipeTitle = fixture.Create<string>()
        };
        var recipeTwo = new RecipeEntity
        {
            RecipeEntityId = fixture.Create<int>(),
            Author = author,
            RecipeCategory = PersistentRecipeCategory.Other,
            RecipeTitle = fixture.Create<string>()
        };
        var recipeThree = new RecipeEntity
        {
            RecipeEntityId = fixture.Create<int>(),
            Author = author,
            RecipeCategory = PersistentRecipeCategory.Other,
            RecipeTitle = fixture.Create<string>()
        };

        RecipeEntity[] recipes =
        {
            recipeOne,
            recipeTwo,
            recipeThree
        };

        using(RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();
            await context.RecipeEntities.AddRangeAsync(recipes);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var repository = new RecipeDataEntryCollectionRepository(context);

            // Call
            IReadOnlyList<RecipeEntryData> entries = await repository.LoadRecipeEntriesByCategoryAsync(RecipeCategory.Dessert);

            // Assert
            entries.Should().BeEmpty();
        }
    }

    [Theory]
    [MemberData(nameof(GetCategoryTests), RecipeCategory.Other)]
    [MemberData(nameof(GetCategoryTests), RecipeCategory.Fish)]
    [MemberData(nameof(GetCategoryTests), RecipeCategory.Meat)]
    [MemberData(nameof(GetCategoryTests), RecipeCategory.Drinks)]
    [MemberData(nameof(GetCategoryTests), RecipeCategory.Pastry)]
    [MemberData(nameof(GetCategoryTests), RecipeCategory.Dessert)]
    [MemberData(nameof(GetCategoryTests), RecipeCategory.Snack)]
    [MemberData(nameof(GetCategoryTests), RecipeCategory.Vegetarian)]
    [MemberData(nameof(GetCategoryTests), RecipeCategory.Vegan)]
    public async Task Given_seeded_database_when_loading_entries_with_category_and_match_found_returns_filtered_data_ordered_by_id(
        IReadOnlyCollection<RecipeEntity> recipeEntries, RecipeCategory categoryToFilter, IEnumerable<RecipeEntity> expectedRecipes)
    {
        // Setup
        using(RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();
            await context.RecipeEntities.AddRangeAsync(recipeEntries);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var repository = new RecipeDataEntryCollectionRepository(context);

            // Call
            IReadOnlyList<RecipeEntryData> entries = await repository.LoadRecipeEntriesByCategoryAsync(categoryToFilter);

            // Assert
            entries.Should().BeInAscendingOrder(s => s.Id).And.BeEquivalentTo(
                expectedRecipes,
                options => options.ExcludingMissingMembers()
                                  .WithAutoConversion()
                                  .WithMapping<RecipeEntryData>(e => e.RecipeEntityId, s => s.Id)
                                  .WithMapping<RecipeEntryData>(e => e.RecipeTitle, s => s.Title)
                                  .WithMapping<AuthorEntity, RecipeEntryData>(e => e.AuthorId, s => s.AuthorId));
        }
    }

    [Theory]
    [MemberData(nameof(GetInvalidAuthorId))]
    public async Task Given_database_with_invalid_author_when_loading_entries_with_category_and_match_found_throws_exception(string invalidAuthorId)
    {
        // Setup
        var invalidAuthorEntity = new AuthorEntity
        {
            AuthorId = invalidAuthorId
        };

        var fixture = new Fixture();
        var recipe = new RecipeEntity
        {
            RecipeEntityId = fixture.Create<int>(),
            Author = invalidAuthorEntity,
            RecipeCategory = PersistentRecipeCategory.Other,
            RecipeTitle = fixture.Create<string>()
        };

        using (RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();
            await context.RecipeEntities.AddAsync(recipe);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var repository = new RecipeDataEntryCollectionRepository(context);

            // Call
            Func<Task> call = () => repository.LoadRecipeEntriesByCategoryAsync(RecipeCategory.Other);

            // Assert
            await call.Should().ThrowAsync<RepositoryDataLoadException>()
                      .WithMessage($"Recipe entries could not be loaded due to invalid AuthorId '{invalidAuthorId}'.");
        }
    }

    [Fact]
    public async Task Given_empty_database_when_loading_entries_by_tag_id_returns_empty_collection()
    {
        // Setup
        var fixture = new Fixture();
        using(RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();

            var repository = new RecipeDataEntryCollectionRepository(context);

            // Call
            IReadOnlyList<RecipeEntryData> entries = await repository.LoadRecipeEntriesByTagIdAsync(fixture.Create<long>());

            // Assert
            entries.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task Given_seeded_database_when_loading_entries_by_tag_id_and_no_match_found_returns_empty_collection()
    {
        // Setup
        using(RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();

            var fixture = new Fixture();
            var author = new AuthorEntity
            {
                AuthorId = fixture.Create<string>()
            };

            var tagEntity = new TagEntity
            {
                TagEntityId = fixture.Create<long>(),
                Tag = fixture.Create<string>()
            };

            var recipe = new RecipeEntity
            {
                RecipeEntityId = 3,
                Author = author,
                RecipeCategory = fixture.Create<PersistentRecipeCategory>(),
                RecipeTitle = fixture.Create<string>(),
                Tags = new[]
                {
                    new RecipeTagEntity
                    {
                        Tag = tagEntity,
                        Order = fixture.Create<byte>()
                    }
                }
            };

            await context.RecipeEntities.AddRangeAsync(recipe);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var repository = new RecipeDataEntryCollectionRepository(context);

            // Call
            IReadOnlyList<RecipeEntryData> entries = await repository.LoadRecipeEntriesByTagIdAsync(fixture.Create<long>());

            // Assert
            entries.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task Given_seeded_database_when_loading_entries_by_tag_id_and_match_found_returns_filtered_data_and_sorted_by_id()
    {
        // Setup
        using(RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();

            var fixture = new Fixture();
            var authorId = fixture.Create<ulong>();
            var author = new AuthorEntity
            {
                AuthorId = authorId.ToString()
            };

            var tagToFilter = new TagEntity
            {
                TagEntityId = fixture.Create<long>(),
                Tag = fixture.Create<string>()
            };
            var tagEntity = new TagEntity
            {
                TagEntityId = fixture.Create<long>(),
                Tag = fixture.Create<string>()
            };

            var recipeOne = new RecipeEntity
            {
                RecipeEntityId = 3,
                Author = author,
                RecipeCategory = fixture.Create<PersistentRecipeCategory>(),
                RecipeTitle = fixture.Create<string>(),
                Tags = new[]
                {
                    new RecipeTagEntity
                    {
                        Tag = tagToFilter,
                        Order = fixture.Create<byte>()
                    }
                }
            };
            var recipeTwo = new RecipeEntity
            {
                RecipeEntityId = 2,
                Author = author,
                RecipeCategory = fixture.Create<PersistentRecipeCategory>(),
                RecipeTitle = fixture.Create<string>(),
                Tags = new[]
                {
                    new RecipeTagEntity
                    {
                        Tag = tagEntity,
                        Order = fixture.Create<byte>()
                    }
                }
            };
            var recipeThree = new RecipeEntity
            {
                RecipeEntityId = 1,
                Author = author,
                RecipeCategory = fixture.Create<PersistentRecipeCategory>(),
                RecipeTitle = fixture.Create<string>(),
                Tags = new[]
                {
                    new RecipeTagEntity
                    {
                        Tag = tagToFilter,
                        Order = fixture.Create<byte>()
                    }
                }
            };

            await context.RecipeEntities.AddRangeAsync(recipeOne, recipeTwo, recipeThree);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var repository = new RecipeDataEntryCollectionRepository(context);

            // Call
            IReadOnlyList<RecipeEntryData> entries = await repository.LoadRecipeEntriesByTagIdAsync(tagToFilter.TagEntityId);

            // Assert
            entries.Should().BeInAscendingOrder(s => s.Id).And.BeEquivalentTo(
                new[]
                {
                    recipeOne,
                    recipeThree
                },
                options => options.ExcludingMissingMembers()
                                  .WithAutoConversion()
                                  .WithMapping<RecipeEntryData>(e => e.RecipeEntityId, s => s.Id)
                                  .WithMapping<RecipeEntryData>(e => e.RecipeTitle, s => s.Title)
                                  .WithMapping<AuthorEntity, RecipeEntryData>(e => e.AuthorId, s => s.AuthorId));
        }
    }

    [Theory]
    [MemberData(nameof(GetInvalidAuthorId))]
    public async Task Given_database_with_invalid_author_when_loading_entries_by_tag_id_and_match_found_throws_exception(string invalidAuthorId)
    {
        // Setup
        using (RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();

            var fixture = new Fixture();
            var invalidAuthorEntity = new AuthorEntity
            {
                AuthorId = invalidAuthorId
            };

            var tagToFilter = new TagEntity
            {
                TagEntityId = fixture.Create<long>(),
                Tag = fixture.Create<string>()
            };

            var recipe = new RecipeEntity
            {
                RecipeEntityId = 3,
                Author = invalidAuthorEntity,
                RecipeCategory = fixture.Create<PersistentRecipeCategory>(),
                RecipeTitle = fixture.Create<string>(),
                Tags = new[]
                {
                    new RecipeTagEntity
                    {
                        Tag = tagToFilter,
                        Order = fixture.Create<byte>()
                    }
                }
            };

            await context.RecipeEntities.AddAsync(recipe);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var repository = new RecipeDataEntryCollectionRepository(context);

            // Call
            Func<Task> call = () => repository.LoadRecipeEntriesByTagIdAsync(tagToFilter.TagEntityId);

            // Assert
            await call.Should().ThrowAsync<RepositoryDataLoadException>()
                      .WithMessage($"Recipe entries could not be loaded due to invalid AuthorId '{invalidAuthorId}'.");
        }
    }

    [Fact]
    public async Task Given_empty_database_when_loading_entries_by_tag_returns_empty_collection()
    {
        // Setup
        var fixture = new Fixture();
        using(RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();

            var repository = new RecipeDataEntryCollectionRepository(context);

            // Call
            IReadOnlyList<RecipeEntryData> entries = await repository.LoadRecipeEntriesByTagAsync(fixture.Create<string>());

            // Assert
            entries.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task Given_seeded_database_when_loading_entries_by_tag_and_no_match_found_returns_empty_collection()
    {
        // Setup
        using(RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();

            var fixture = new Fixture();
            var author = new AuthorEntity
            {
                AuthorId = fixture.Create<string>()
            };

            var tagEntity = new TagEntity
            {
                TagEntityId = fixture.Create<long>(),
                Tag = fixture.Create<string>()
            };

            var recipe = new RecipeEntity
            {
                RecipeEntityId = 3,
                Author = author,
                RecipeCategory = fixture.Create<PersistentRecipeCategory>(),
                RecipeTitle = fixture.Create<string>(),
                Tags = new[]
                {
                    new RecipeTagEntity
                    {
                        Tag = tagEntity,
                        Order = fixture.Create<byte>()
                    }
                }
            };

            await context.RecipeEntities.AddAsync(recipe);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var repository = new RecipeDataEntryCollectionRepository(context);

            // Call
            IReadOnlyList<RecipeEntryData> entries = await repository.LoadRecipeEntriesByTagAsync(fixture.Create<string>());

            // Assert
            entries.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task Given_seeded_database_when_loading_entries_by_tag_and_match_found_returns_filtered_data_and_sorted_by_id()
    {
        // Setup
        using(RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();

            var fixture = new Fixture();
            var authorId = fixture.Create<ulong>();
            var author = new AuthorEntity
            {
                AuthorId = authorId.ToString()
            };

            var tagToFilter = new TagEntity
            {
                TagEntityId = fixture.Create<long>(),
                Tag = fixture.Create<string>()
            };
            var tagEntity = new TagEntity
            {
                TagEntityId = fixture.Create<long>(),
                Tag = fixture.Create<string>()
            };

            var recipeOne = new RecipeEntity
            {
                RecipeEntityId = 3,
                Author = author,
                RecipeCategory = fixture.Create<PersistentRecipeCategory>(),
                RecipeTitle = fixture.Create<string>(),
                Tags = new[]
                {
                    new RecipeTagEntity
                    {
                        Tag = tagToFilter,
                        Order = fixture.Create<byte>()
                    }
                }
            };
            var recipeTwo = new RecipeEntity
            {
                RecipeEntityId = 2,
                Author = author,
                RecipeCategory = fixture.Create<PersistentRecipeCategory>(),
                RecipeTitle = fixture.Create<string>(),
                Tags = new[]
                {
                    new RecipeTagEntity
                    {
                        Tag = tagEntity,
                        Order = fixture.Create<byte>()
                    }
                }
            };
            var recipeThree = new RecipeEntity
            {
                RecipeEntityId = 1,
                Author = author,
                RecipeCategory = fixture.Create<PersistentRecipeCategory>(),
                RecipeTitle = fixture.Create<string>(),
                Tags = new[]
                {
                    new RecipeTagEntity
                    {
                        Tag = tagToFilter,
                        Order = fixture.Create<byte>()
                    }
                }
            };

            await context.RecipeEntities.AddRangeAsync(recipeOne, recipeTwo, recipeThree);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var repository = new RecipeDataEntryCollectionRepository(context);

            // Call
            IReadOnlyList<RecipeEntryData> entries = await repository.LoadRecipeEntriesByTagAsync(tagToFilter.Tag);

            // Assert
            entries.Should().BeInAscendingOrder(s => s.Id).And.BeEquivalentTo(
                new[]
                {
                    recipeOne,
                    recipeThree
                },
                options => options.ExcludingMissingMembers()
                                  .WithAutoConversion()
                                  .WithMapping<RecipeEntryData>(e => e.RecipeEntityId, s => s.Id)
                                  .WithMapping<RecipeEntryData>(e => e.RecipeTitle, s => s.Title)
                                  .WithMapping<AuthorEntity, RecipeEntryData>(e => e.AuthorId, s => s.AuthorId));
        }
    }

    [Theory]
    [MemberData(nameof(GetInvalidAuthorId))]
    public async Task Given_database_with_invalid_author_when_loading_entries_by_tag_and_match_found_throws_exception(string invalidAuthorId)
    {
        // Setup
        using (RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();

            var fixture = new Fixture();
            var invalidAuthorEntity = new AuthorEntity
            {
                AuthorId = invalidAuthorId
            };

            var tagToFilter = new TagEntity
            {
                TagEntityId = fixture.Create<long>(),
                Tag = fixture.Create<string>()
            };

            var recipe = new RecipeEntity
            {
                RecipeEntityId = 3,
                Author = invalidAuthorEntity,
                RecipeCategory = fixture.Create<PersistentRecipeCategory>(),
                RecipeTitle = fixture.Create<string>(),
                Tags = new[]
                {
                    new RecipeTagEntity
                    {
                        Tag = tagToFilter,
                        Order = fixture.Create<byte>()
                    }
                }
            };

            await context.RecipeEntities.AddAsync(recipe);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var repository = new RecipeDataEntryCollectionRepository(context);

            // Call
            Func<Task> call = () => repository.LoadRecipeEntriesByTagAsync(tagToFilter.Tag);

            // Assert
            await call.Should().ThrowAsync<RepositoryDataLoadException>()
                      .WithMessage($"Recipe entries could not be loaded due to invalid AuthorId '{invalidAuthorId}'.");
        }
    }

    [Fact]
    public async Task Given_empty_database_when_loading_entries_by_author_id_returns_empty_collection()
    {
        // Given
        using (RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();

            var fixture = new Fixture();
            var authorId = fixture.Create<ulong>();
            

            var repository = new RecipeDataEntryCollectionRepository(context);

            // Call
            IReadOnlyList<RecipeEntryData> entries = await repository.LoadRecipeEntriesByAuthorIdAsync(authorId);

            // Assert
            entries.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task Given_seeded_database_when_loading_entries_by_author_id_and_match_found_returns_filtered_and_sorted_by_id()
    {
        // Given
        using (RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();

            var fixture = new Fixture();
            var authorId = fixture.Create<ulong>();
            var authorToRetrieve = new AuthorEntity
            {
                AuthorId = authorId.ToString()
            };
            var author = new AuthorEntity
            {
                AuthorId = fixture.Create<long>().ToString()
            };

            var recipeOne = new RecipeEntity
            {
                RecipeEntityId = 3,
                Author = authorToRetrieve,
                RecipeCategory = fixture.Create<PersistentRecipeCategory>(),
                RecipeTitle = fixture.Create<string>(),
                Tags = Array.Empty<RecipeTagEntity>()
            };
            var recipeTwo = new RecipeEntity
            {
                RecipeEntityId = 2,
                Author = author,
                RecipeCategory = fixture.Create<PersistentRecipeCategory>(),
                RecipeTitle = fixture.Create<string>(),
                Tags = Array.Empty<RecipeTagEntity>()
            };
            var recipeThree = new RecipeEntity
            {
                RecipeEntityId = 1,
                Author = authorToRetrieve,
                RecipeCategory = fixture.Create<PersistentRecipeCategory>(),
                RecipeTitle = fixture.Create<string>(),
                Tags = Array.Empty<RecipeTagEntity>()
            };

            await context.RecipeEntities.AddRangeAsync(recipeOne, recipeTwo, recipeThree);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var repository = new RecipeDataEntryCollectionRepository(context);

            // Call
            IReadOnlyList<RecipeEntryData> entries = await repository.LoadRecipeEntriesByAuthorIdAsync(authorId);

            // Assert
            entries.Should().BeInAscendingOrder(s => s.Id).And.BeEquivalentTo(
                new[]
                {
                    recipeOne,
                    recipeThree
                },
                options => options.ExcludingMissingMembers()
                                  .WithAutoConversion()
                                  .WithMapping<RecipeEntryData>(e => e.RecipeEntityId, s => s.Id)
                                  .WithMapping<RecipeEntryData>(e => e.RecipeTitle, s => s.Title)
                                  .WithMapping<AuthorEntity, RecipeEntryData>(e => e.AuthorId, s => s.AuthorId));
        }

    }

    public static IEnumerable<object[]> GetCategoryTests(RecipeCategory categoryToFilter)
    {
        PersistentRecipeCategory persistentRecipeCategoryToFilter = PersistentRecipeCategoryCreator.Create(categoryToFilter);
        IReadOnlyCollection<RecipeEntity> databaseEntries = GetDatabaseSeed(persistentRecipeCategoryToFilter);
        yield return new object[]
        {
            databaseEntries,
            categoryToFilter,
            databaseEntries.Where(e => e.RecipeCategory == persistentRecipeCategoryToFilter).OrderBy(e => e.RecipeEntityId)
        };
    }

    public static IEnumerable<object[]> GetInvalidAuthorId()
    {
        yield return new object[]
        {
            "X"
        };
        yield return new object[]
        {
            "18446744073709551616"
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        connection.Dispose();
    }

    private static IReadOnlyCollection<RecipeEntity> GetDatabaseSeed(PersistentRecipeCategory category)
    {
        var fixture = new Fixture();
        var authorOne = new AuthorEntity
        {
            AuthorId = fixture.Create<ulong>().ToString()
        };
        var authorTwo = new AuthorEntity
        {
            AuthorId = fixture.Create<ulong>().ToString()
        };

        var recipeOne = new RecipeEntity
        {
            RecipeEntityId = fixture.Create<int>(),
            Author = authorOne,
            RecipeCategory = category,
            RecipeTitle = fixture.Create<string>()
        };
        var recipeTwo = new RecipeEntity
        {
            RecipeEntityId = fixture.Create<int>(),
            Author = authorTwo,
            RecipeCategory = category == PersistentRecipeCategory.Other ? PersistentRecipeCategory.Dessert : PersistentRecipeCategory.Other,
            RecipeTitle = fixture.Create<string>()
        };
        var recipeThree = new RecipeEntity
        {
            RecipeEntityId = fixture.Create<int>(),
            Author = authorTwo,
            RecipeCategory = category,
            RecipeTitle = fixture.Create<string>()
        };

        return new[]
        {
            recipeOne,
            recipeTwo,
            recipeThree
        };
    }

    private RecipeBotDbContext CreateContext()
    {
        DbContextOptions<RecipeBotDbContext> contextOptions =
            new DbContextOptionsBuilder<RecipeBotDbContext>().UseSqlite(connection)
                                                             .Options;
        return new RecipeBotDbContext(contextOptions);
    }
}