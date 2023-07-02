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
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Repositories;
using RecipeBot.Domain.Repositories.Data;
using RecipeBot.Persistence.Entities;
using Xunit;

namespace RecipeBot.Persistence.Test;

public class AuthorRepositoryTest : IDisposable
{
    private readonly SqliteConnection connection;

    public AuthorRepositoryTest()
    {
        connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
    }

    [Fact]
    public void Repository_is_recipe_repository()
    {
        // Setup
        using(var context = new RecipeBotDbContext(new DbContextOptions<RecipeBotDbContext>()))
        {
            // Call
            var repository = new AuthorRepository(context);

            // Assert
            repository.Should().BeAssignableTo<IAuthorRepository>();
        }
    }

    [Fact]
    public async Task Given_empty_database_when_deleting_author_throws_exception()
    {
        // Setup
        var fixture = new Fixture();

        using(RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();

            var repository = new AuthorRepository(context);

            // Call
            Func<Task> call = () => repository.DeleteEntityAsync(fixture.Create<ulong>());

            // Assert
            await call.Should().ThrowAsync<RepositoryDataDeleteException>()
                      .WithMessage("No matching author found.");
        }
    }

    [Fact]
    public async Task Given_seeded_database_when_deleting_author_only_deletes_affected_data()
    {
        // Setup
        using(RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();

            var fixture = new Fixture();
            var authorId = fixture.Create<ulong>();
            var authorToDelete = new AuthorEntity
            {
                AuthorId = authorId.ToString()
            };
            var author = new AuthorEntity
            {
                AuthorId = fixture.Create<ulong>().ToString()
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
                Author = authorToDelete,
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
                Author = author,
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

            var repository = new AuthorRepository(context);

            // Call
            await repository.DeleteEntityAsync(authorId);
            context.ChangeTracker.Clear();

            // Assert
            context.AuthorEntities.Should().BeEquivalentTo(new[]
            {
                author
            }, options => options.Excluding(s => s.Recipes));

            context.TagEntities.Should().BeEquivalentTo(tagEntities, options => options.Excluding(s => s.Recipes));
            context.RecipeFieldEntities.Should().BeEquivalentTo(unaffectedRecipe.RecipeFields, options => options.Excluding(s => s.Recipe));
            context.RecipeTagEntities.Should().BeEquivalentTo(unaffectedRecipe.Tags, options => options.Excluding(s => s.Recipe)
                                                                                                       .Excluding(s => s.Tag));

            RecipeEntity recipeEntity = await context.RecipeEntities
                                                     .Include(e => e.Author)
                                                     .Include(e => e.RecipeFields)
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
    public async Task Given_empty_database_when_deleting_author_by_entity_id_throws_exception()
    {
        // Setup
        var fixture = new Fixture();

        using (RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();

            var repository = new AuthorRepository(context);

            // Call
            Func<Task> call = () => repository.DeleteAuthorAsync(fixture.Create<long>());

            // Assert
            await call.Should().ThrowAsync<RepositoryDataDeleteException>()
                      .WithMessage("No matching author found.");
        }
    }

    [Fact]
    public async Task Given_seeded_database_when_deleting_author_by_entity_id_only_deletes_affected_data()
    {
        // Setup
        using (RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();

            var fixture = new Fixture();
            var authorId = fixture.Create<ulong>();
            var authorToDelete = new AuthorEntity
            {
                AuthorId = authorId.ToString()
            };
            var author = new AuthorEntity
            {
                AuthorId = fixture.Create<ulong>().ToString()
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
                Author = authorToDelete,
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
                Author = author,
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

            var repository = new AuthorRepository(context);

            // Call
            await repository.DeleteAuthorAsync(authorToDelete.AuthorEntityId);
            context.ChangeTracker.Clear();

            // Assert
            context.AuthorEntities.Should().BeEquivalentTo(new[]
            {
                author
            }, options => options.Excluding(s => s.Recipes));

            context.TagEntities.Should().BeEquivalentTo(tagEntities, options => options.Excluding(s => s.Recipes));
            context.RecipeFieldEntities.Should().BeEquivalentTo(unaffectedRecipe.RecipeFields, options => options.Excluding(s => s.Recipe));
            context.RecipeTagEntities.Should().BeEquivalentTo(unaffectedRecipe.Tags, options => options.Excluding(s => s.Recipe)
                                                                                                       .Excluding(s => s.Tag));

            RecipeEntity recipeEntity = await context.RecipeEntities
                                                     .Include(e => e.Author)
                                                     .Include(e => e.RecipeFields)
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
    public async Task Given_empty_database_when_loading_authors_returns_empty_collection()
    {
        // Setup
        using(RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();

            var repository = new AuthorRepository(context);

            // Call
            IReadOnlyCollection<AuthorRepositoryEntityData> authors = await repository.LoadAuthorsAsync();

            // Assert
            authors.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task Given_seeded_database_when_loading_authors_returns_collection_sorted_by_entity_id()
    {
        // Setup
        using(RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();

            var fixture = new Fixture();
            var authorOne = new AuthorEntity
            {
                AuthorEntityId = 1,
                AuthorId = fixture.Create<ulong>().ToString()
            };
            var authorTwo = new AuthorEntity
            {
                AuthorEntityId = 3,
                AuthorId = fixture.Create<ulong>().ToString()
            };
            var authorThree = new AuthorEntity
            {
                AuthorEntityId = 2,
                AuthorId = fixture.Create<ulong>().ToString()
            };

            AuthorEntity[] authorEntities =
            {
                authorOne,
                authorTwo,
                authorThree
            };

            await context.AuthorEntities.AddRangeAsync(authorEntities);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var repository = new AuthorRepository(context);

            // Call
            IReadOnlyCollection<AuthorRepositoryEntityData> authors = await repository.LoadAuthorsAsync();

            // Assert
            authors.Should().BeInAscendingOrder(s => s.EntityId).And.BeEquivalentTo(
                authorEntities,
                options => options.ExcludingMissingMembers()
                                  .WithAutoConversion()
                                  .WithMapping<AuthorRepositoryEntityData>(e => e.AuthorEntityId, s => s.EntityId)
                                  .WithMapping<AuthorRepositoryEntityData>(e => e.AuthorId, s => s.AuthorId));
        }
    }

    [Theory]
    [InlineData("X")]
    [InlineData("18446744073709551616")]
    public async Task Given_seeded_database_when_loading_authors_with_invalid_author_id_throws_exception(string invalidAuthorId)
    {
        // Setup
        using(RecipeBotDbContext context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();

            var fixture = new Fixture();
            var validAuthor = new AuthorEntity
            {
                AuthorId = fixture.Create<ulong>().ToString()
            };
            var invalidAuthor = new AuthorEntity
            {
                AuthorId = invalidAuthorId
            };

            AuthorEntity[] authorEntities =
            {
                validAuthor,
                invalidAuthor
            };

            await context.AuthorEntities.AddRangeAsync(authorEntities);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var repository = new AuthorRepository(context);

            // Call
            Func<Task> call = () => repository.LoadAuthorsAsync();

            // Assert
            await call.Should().ThrowAsync<RepositoryDataLoadException>()
                      .WithMessage($"Author entries could not be loaded due to invalid AuthorId '{invalidAuthor.AuthorId}'.");
        }
    }

    public void Dispose()
    {
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