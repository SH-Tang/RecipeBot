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
using Discord;
using Discord.Common.Providers;
using Discord.Common.Services;
using Discord.Common.TestUtils;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using RecipeBot.Controllers;
using RecipeBot.Discord.Controllers;
using RecipeBot.Discord.Data;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Repositories;
using RecipeBot.Domain.Repositories.Data;
using RecipeBot.TestUtils;
using Xunit;

namespace RecipeBot.Test.Controllers;

public class RecipeEntriesControllerTest
{
    [Fact]
    public void Controller_is_recipe_entries_controller()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        var repository = Substitute.For<IRecipeCollectionRepository>();
        var logger = Substitute.For<ILoggingService>();

        // Call
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Assert
        controller.Should().BeAssignableTo<IRecipeEntriesController>();
    }

    [Fact]
    public async Task Given_repository_returning_empty_collection_when_listing_all_recipes_returns_expected_message()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var userDataProvider = Substitute.For<IUserDataProvider>();

        var repository = Substitute.For<IRecipeCollectionRepository>();
        repository.LoadRecipeEntriesAsync().ReturnsForAnyArgs(Array.Empty<RecipeRepositoryEntityData>());

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesAsync();

        // Assert
        result.HasError.Should().BeFalse();

        result.Result.Should().HaveCount(1).And.Contain("No saved recipes are found.");
    }

    [Fact]
    public async Task Given_repository_returning_collection_without_distinct_authors_when_listing_all_recipes_returns_expected_message()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(int.MaxValue);

        var fixture = new Fixture();
        var authorId = fixture.Create<ulong>();

        var entries = new[]
        {
            new RecipeRepositoryEntityData(fixture.Create<long>(), fixture.Create<string>(), authorId),
            new RecipeRepositoryEntityData(fixture.Create<long>(), fixture.Create<string>(), authorId)
        };

        var repository = Substitute.For<IRecipeCollectionRepository>();
        repository.LoadRecipeEntriesAsync().ReturnsForAnyArgs(entries);

        var userName = fixture.Create<string>();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(authorId).Returns(UserDataTestFactory.Create(userName));

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesAsync();

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessage =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].EntityId,-3} {entries[0].Title,-50} {userName,-50}{Environment.NewLine}" +
            $"{entries[1].EntityId,-3} {entries[1].Title,-50} {userName,-50}{Environment.NewLine}";
        result.Result.Should().HaveCount(1).And.Contain(Format.Code(expectedMessage));
    }

    [Fact]
    public async Task Given_repository_returning_collection_and_message_within_limits_when_listing_all_recipes_returns_expected_message()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(int.MaxValue);

        var fixture = new Fixture();
        RecipeRepositoryEntityData[] entries = fixture.CreateMany<RecipeRepositoryEntityData>(3).ToArray();

        var repository = Substitute.For<IRecipeCollectionRepository>();
        repository.LoadRecipeEntriesAsync().ReturnsForAnyArgs(entries);

        IReadOnlyList<UserData> userData = GetUsers(fixture, entries.Length);
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(entries[0].AuthorId).Returns(userData[0]);
        userDataProvider.GetUserDataAsync(entries[1].AuthorId).Returns(userData[1]);
        userDataProvider.GetUserDataAsync(entries[2].AuthorId).Returns(userData[2]);

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesAsync();

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessage =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].EntityId,-3} {entries[0].Title,-50} {userData[0].Username,-50}{Environment.NewLine}" +
            $"{entries[1].EntityId,-3} {entries[1].Title,-50} {userData[1].Username,-50}{Environment.NewLine}" +
            $"{entries[2].EntityId,-3} {entries[2].Title,-50} {userData[2].Username,-50}{Environment.NewLine}";
        result.Result.Should().HaveCount(1).And.Contain(Format.Code(expectedMessage));
    }

    [Theory]
    [InlineData(328)] // Exactly intersects with 2 entries
    [InlineData(340)]
    public async Task Given_repository_returning_collection_and_message_exceeding_limits_when_listing_all_recipes_returns_expected_messages(
        int maxMessageLength)
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(maxMessageLength);

        var fixture = new Fixture();
        RecipeRepositoryEntityData[] entries = fixture.CreateMany<RecipeRepositoryEntityData>(3).ToArray();

        IReadOnlyList<UserData> userData = GetUsers(fixture, entries.Length);
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(entries[0].AuthorId).Returns(userData[0]);
        userDataProvider.GetUserDataAsync(entries[1].AuthorId).Returns(userData[1]);
        userDataProvider.GetUserDataAsync(entries[2].AuthorId).Returns(userData[2]);

        var repository = Substitute.For<IRecipeCollectionRepository>();
        repository.LoadRecipeEntriesAsync().ReturnsForAnyArgs(entries);

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesAsync();

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessageOne =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].EntityId,-3} {entries[0].Title,-50} {userData[0].Username,-50}{Environment.NewLine}" +
            $"{entries[1].EntityId,-3} {entries[1].Title,-50} {userData[1].Username,-50}{Environment.NewLine}";

        string expectedMessageTwo =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[2].EntityId,-3} {entries[2].Title,-50} {userData[2].Username,-50}{Environment.NewLine}";

        result.Result.Should().BeEquivalentTo(new[]
        {
            Format.Code(expectedMessageOne),
            Format.Code(expectedMessageTwo)
        }, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task Given_repository_throwing_exception_when_listing_all_recipes_then_logs_and_returns_result_with_error()
    {
        // Setup
        var fixture = new Fixture();
        var exceptionMessage = fixture.Create<string>();

        var repository = Substitute.For<IRecipeCollectionRepository>();
        var exception = new RepositoryDataLoadException(exceptionMessage);
        repository.LoadRecipeEntriesAsync().ThrowsAsyncForAnyArgs(exception);

        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesAsync();

        // Assert
        result.HasError.Should().BeTrue();
        result.ErrorMessage.Should().Be(exceptionMessage);

        logger.Received(1).LogError(exception);
    }

    [Fact]
    public async Task Given_repository_returning_empty_collection_when_filtering_recipes_by_category_returns_expected_message()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var userDataProvider = Substitute.For<IUserDataProvider>();

        var repository = Substitute.For<IRecipeCollectionRepository>();
        repository.LoadRecipeEntriesByCategoryAsync(Arg.Any<RecipeCategory>()).ReturnsForAnyArgs(Array.Empty<RecipeRepositoryEntityData>());

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        var fixture = new Fixture();

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByCategoryAsync(fixture.Create<DiscordRecipeCategory>());

        // Assert
        result.HasError.Should().BeFalse();

        result.Result.Should().HaveCount(1).And.Contain("No saved recipes are found with the given category.");
    }

    [Theory]
    [InlineData(DiscordRecipeCategory.Meat)]
    [InlineData(DiscordRecipeCategory.Fish)]
    [InlineData(DiscordRecipeCategory.Vegetarian)]
    [InlineData(DiscordRecipeCategory.Vegan)]
    [InlineData(DiscordRecipeCategory.Drinks)]
    [InlineData(DiscordRecipeCategory.Pastry)]
    [InlineData(DiscordRecipeCategory.Dessert)]
    [InlineData(DiscordRecipeCategory.Snack)]
    [InlineData(DiscordRecipeCategory.Other)]
    public async Task Filtering_recipes_by_category_repository_receives_correct_category(DiscordRecipeCategory category)
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var userDataProvider = Substitute.For<IUserDataProvider>();

        var repository = Substitute.For<IRecipeCollectionRepository>();
        repository.LoadRecipeEntriesByCategoryAsync(Arg.Any<RecipeCategory>()).ReturnsForAnyArgs(Array.Empty<RecipeRepositoryEntityData>());

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        await controller.GetAllRecipesByCategoryAsync(category);

        // Assert
        await repository.Received(1).LoadRecipeEntriesByCategoryAsync(DiscordRecipeCategoryTestHelper.RecipeCategoryMapping[category]);
    }

    [Fact]
    public async Task Given_repository_returning_collection_without_distinct_author_when_filtering_recipes_by_category_returns_expected_message()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(int.MaxValue);

        var fixture = new Fixture();
        var authorId = fixture.Create<ulong>();

        var entries = new[]
        {
            new RecipeRepositoryEntityData(fixture.Create<long>(), fixture.Create<string>(), authorId),
            new RecipeRepositoryEntityData(fixture.Create<long>(), fixture.Create<string>(), authorId)
        };

        var userName = fixture.Create<string>();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(authorId).Returns(UserDataTestFactory.Create(userName));

        var repository = Substitute.For<IRecipeCollectionRepository>();
        repository.LoadRecipeEntriesByCategoryAsync(Arg.Any<RecipeCategory>()).ReturnsForAnyArgs(entries);

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByCategoryAsync(fixture.Create<DiscordRecipeCategory>());

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessage =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].EntityId,-3} {entries[0].Title,-50} {userName,-50}{Environment.NewLine}" +
            $"{entries[1].EntityId,-3} {entries[1].Title,-50} {userName,-50}{Environment.NewLine}";
        result.Result.Should().HaveCount(1).And.Contain(Format.Code(expectedMessage));
    }

    [Fact]
    public async Task Given_repository_returning_collection_and_message_within_limits_when_filtering_recipes_by_category_returns_expected_message()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(int.MaxValue);

        var fixture = new Fixture();
        RecipeRepositoryEntityData[] entries = fixture.CreateMany<RecipeRepositoryEntityData>(3).ToArray();

        IReadOnlyList<UserData> userData = GetUsers(fixture, entries.Length);
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(entries[0].AuthorId).Returns(userData[0]);
        userDataProvider.GetUserDataAsync(entries[1].AuthorId).Returns(userData[1]);
        userDataProvider.GetUserDataAsync(entries[2].AuthorId).Returns(userData[2]);

        var repository = Substitute.For<IRecipeCollectionRepository>();
        repository.LoadRecipeEntriesByCategoryAsync(Arg.Any<RecipeCategory>()).ReturnsForAnyArgs(entries);

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByCategoryAsync(fixture.Create<DiscordRecipeCategory>());

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessage =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].EntityId,-3} {entries[0].Title,-50} {userData[0].Username,-50}{Environment.NewLine}" +
            $"{entries[1].EntityId,-3} {entries[1].Title,-50} {userData[1].Username,-50}{Environment.NewLine}" +
            $"{entries[2].EntityId,-3} {entries[2].Title,-50} {userData[2].Username,-50}{Environment.NewLine}";
        result.Result.Should().HaveCount(1).And.Contain(Format.Code(expectedMessage));
    }

    [Theory]
    [InlineData(328)] // Exactly intersects with 2 entries
    [InlineData(340)]
    public async Task Given_repository_returning_collection_and_message_exceeding_limits_when_filtering_recipes_by_category_returns_expected_messages(
        int maxMessageLength)
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(maxMessageLength);

        var fixture = new Fixture();
        RecipeRepositoryEntityData[] entries = fixture.CreateMany<RecipeRepositoryEntityData>(3).ToArray();

        IReadOnlyList<UserData> userData = GetUsers(fixture, entries.Length);
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(entries[0].AuthorId).Returns(userData[0]);
        userDataProvider.GetUserDataAsync(entries[1].AuthorId).Returns(userData[1]);
        userDataProvider.GetUserDataAsync(entries[2].AuthorId).Returns(userData[2]);

        var repository = Substitute.For<IRecipeCollectionRepository>();
        repository.LoadRecipeEntriesByCategoryAsync(Arg.Any<RecipeCategory>()).ReturnsForAnyArgs(entries);

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByCategoryAsync(fixture.Create<DiscordRecipeCategory>());

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessageOne =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].EntityId,-3} {entries[0].Title,-50} {userData[0].Username,-50}{Environment.NewLine}" +
            $"{entries[1].EntityId,-3} {entries[1].Title,-50} {userData[1].Username,-50}{Environment.NewLine}";

        string expectedMessageTwo =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[2].EntityId,-3} {entries[2].Title,-50} {userData[2].Username,-50}{Environment.NewLine}";

        result.Result.Should().BeEquivalentTo(new[]
        {
            Format.Code(expectedMessageOne),
            Format.Code(expectedMessageTwo)
        }, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task Given_repository_throwing_exception_when_listing_recipes_by_category_then_logs_and_returns_result_with_error()
    {
        // Setup
        var fixture = new Fixture();
        var exceptionMessage = fixture.Create<string>();

        var repository = Substitute.For<IRecipeCollectionRepository>();
        var exception = new RepositoryDataLoadException(exceptionMessage);
        repository.LoadRecipeEntriesByCategoryAsync(Arg.Any<RecipeCategory>()).ThrowsAsyncForAnyArgs(exception);

        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByCategoryAsync(fixture.Create<DiscordRecipeCategory>());

        // Assert
        result.HasError.Should().BeTrue();
        result.ErrorMessage.Should().Be(exceptionMessage);

        logger.Received(1).LogError(exception);
    }

    [Fact]
    public async Task Given_repository_returning_empty_collection_when_filtering_recipes_by_tag_returns_expected_message()
    {
        // Setup
        var fixture = new Fixture();
        var tagToFilter = fixture.Create<string>();

        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var userDataProvider = Substitute.For<IUserDataProvider>();

        var repository = Substitute.For<IRecipeCollectionRepository>();
        repository.LoadRecipeEntriesByTagAsync(Arg.Any<string>()).ReturnsForAnyArgs(Array.Empty<RecipeRepositoryEntityData>());

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByTagAsync(tagToFilter);

        // Assert
        result.HasError.Should().BeFalse();

        result.Result.Should().HaveCount(1).And.Contain($"No saved recipes are found with the tag '{tagToFilter}'.");
    }

    [Theory]
    [InlineData("Tag to filter")]
    [InlineData("tagToFilter")]
    [InlineData("  TagToFilter")]
    [InlineData("TagToFilter      ")]
    public async Task Filtering_recipes_by_tag_repository_receives_correct_tag(string tag)
    {
        // Setup
        const string expectedTagArgument = "tagtofilter";

        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var userDataProvider = Substitute.For<IUserDataProvider>();

        var repository = Substitute.For<IRecipeCollectionRepository>();
        repository.LoadRecipeEntriesByTagAsync(expectedTagArgument).ReturnsForAnyArgs(Array.Empty<RecipeRepositoryEntityData>());

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        await controller.GetAllRecipesByTagAsync(tag);

        // Assert
        await repository.Received(1).LoadRecipeEntriesByTagAsync(expectedTagArgument);
    }

    [Fact]
    public async Task Given_repository_returning_collection_without_distinct_author_when_filtering_recipes_by_tag_returns_expected_message()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(int.MaxValue);

        var fixture = new Fixture();
        var authorId = fixture.Create<ulong>();

        var entries = new[]
        {
            new RecipeRepositoryEntityData(fixture.Create<long>(), fixture.Create<string>(), authorId),
            new RecipeRepositoryEntityData(fixture.Create<long>(), fixture.Create<string>(), authorId)
        };

        var userName = fixture.Create<string>();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(authorId).Returns(UserDataTestFactory.Create(userName));

        var repository = Substitute.For<IRecipeCollectionRepository>();
        repository.LoadRecipeEntriesByTagAsync(Arg.Any<string>()).ReturnsForAnyArgs(entries);

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByTagAsync(fixture.Create<string>());

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessage =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].EntityId,-3} {entries[0].Title,-50} {userName,-50}{Environment.NewLine}" +
            $"{entries[1].EntityId,-3} {entries[1].Title,-50} {userName,-50}{Environment.NewLine}";
        result.Result.Should().HaveCount(1).And.Contain(Format.Code(expectedMessage));
    }

    [Fact]
    public async Task Given_repository_returning_collection_and_message_within_limits_when_filtering_recipes_by_tag_returns_expected_message()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(int.MaxValue);

        var fixture = new Fixture();
        RecipeRepositoryEntityData[] entries = fixture.CreateMany<RecipeRepositoryEntityData>(3).ToArray();

        IReadOnlyList<UserData> userData = GetUsers(fixture, entries.Length);
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(entries[0].AuthorId).Returns(userData[0]);
        userDataProvider.GetUserDataAsync(entries[1].AuthorId).Returns(userData[1]);
        userDataProvider.GetUserDataAsync(entries[2].AuthorId).Returns(userData[2]);

        var repository = Substitute.For<IRecipeCollectionRepository>();
        repository.LoadRecipeEntriesByTagAsync(Arg.Any<string>()).ReturnsForAnyArgs(entries);

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByTagAsync(fixture.Create<string>());

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessage =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].EntityId,-3} {entries[0].Title,-50} {userData[0].Username,-50}{Environment.NewLine}" +
            $"{entries[1].EntityId,-3} {entries[1].Title,-50} {userData[1].Username,-50}{Environment.NewLine}" +
            $"{entries[2].EntityId,-3} {entries[2].Title,-50} {userData[2].Username,-50}{Environment.NewLine}";
        result.Result.Should().HaveCount(1).And.Contain(Format.Code(expectedMessage));
    }

    [Theory]
    [InlineData(328)] // Exactly intersects with 2 entries
    [InlineData(340)]
    public async Task Given_repository_returning_collection_and_message_exceeding_limits_when_filtering_recipes_by_tag_returns_expected_messages(
        int maxMessageLength)
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(maxMessageLength);

        var fixture = new Fixture();
        RecipeRepositoryEntityData[] entries = fixture.CreateMany<RecipeRepositoryEntityData>(3).ToArray();

        IReadOnlyList<UserData> userData = GetUsers(fixture, entries.Length);
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(entries[0].AuthorId).Returns(userData[0]);
        userDataProvider.GetUserDataAsync(entries[1].AuthorId).Returns(userData[1]);
        userDataProvider.GetUserDataAsync(entries[2].AuthorId).Returns(userData[2]);

        var repository = Substitute.For<IRecipeCollectionRepository>();
        repository.LoadRecipeEntriesByTagAsync(Arg.Any<string>()).ReturnsForAnyArgs(entries);

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByTagAsync(fixture.Create<string>());

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessageOne =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].EntityId,-3} {entries[0].Title,-50} {userData[0].Username,-50}{Environment.NewLine}" +
            $"{entries[1].EntityId,-3} {entries[1].Title,-50} {userData[1].Username,-50}{Environment.NewLine}";

        string expectedMessageTwo =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[2].EntityId,-3} {entries[2].Title,-50} {userData[2].Username,-50}{Environment.NewLine}";

        result.Result.Should().BeEquivalentTo(new[]
        {
            Format.Code(expectedMessageOne),
            Format.Code(expectedMessageTwo)
        }, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task Given_repository_throwing_exception_when_listing_by_tag_then_logs_and_returns_result_with_error()
    {
        // Setup
        var fixture = new Fixture();
        var exceptionMessage = fixture.Create<string>();

        var repository = Substitute.For<IRecipeCollectionRepository>();
        var exception = new RepositoryDataLoadException(exceptionMessage);
        repository.LoadRecipeEntriesByTagAsync(Arg.Any<string>()).ThrowsAsyncForAnyArgs(exception);

        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByTagAsync(fixture.Create<string>());

        // Assert
        result.HasError.Should().BeTrue();
        result.ErrorMessage.Should().Be(exceptionMessage);

        logger.Received(1).LogError(exception);
    }

    [Fact]
    public async Task Given_repository_returning_empty_collection_when_filtering_recipes_by_tag_id_returns_expected_message()
    {
        // Setup
        var fixture = new Fixture();
        var idToFilter = fixture.Create<long>();

        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var userDataProvider = Substitute.For<IUserDataProvider>();

        var repository = Substitute.For<IRecipeCollectionRepository>();
        repository.LoadRecipeEntriesByTagEntityIdAsync(Arg.Any<long>()).ReturnsForAnyArgs(Array.Empty<RecipeRepositoryEntityData>());

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByTagIdAsync(idToFilter);

        // Assert
        result.HasError.Should().BeFalse();

        result.Result.Should().HaveCount(1).And.Contain($"No saved recipes are found with the tag id '{idToFilter}'.");
    }

    [Fact]
    public async Task Filtering_recipes_by_tag_id_repository_receives_correct_tag_id()
    {
        // Setup
        var fixture = new Fixture();
        var idToFilter = fixture.Create<long>();

        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var userDataProvider = Substitute.For<IUserDataProvider>();

        var repository = Substitute.For<IRecipeCollectionRepository>();
        repository.LoadRecipeEntriesByTagEntityIdAsync(idToFilter).ReturnsForAnyArgs(Array.Empty<RecipeRepositoryEntityData>());

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        await controller.GetAllRecipesByTagIdAsync(idToFilter);

        // Assert
        await repository.Received(1).LoadRecipeEntriesByTagEntityIdAsync(idToFilter);
    }

    [Fact]
    public async Task Given_repository_returning_collection_without_distinct_author_when_filtering_recipes_by_tag_id_returns_expected_message()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(int.MaxValue);

        var fixture = new Fixture();
        var authorId = fixture.Create<ulong>();

        var entries = new[]
        {
            new RecipeRepositoryEntityData(fixture.Create<long>(), fixture.Create<string>(), authorId),
            new RecipeRepositoryEntityData(fixture.Create<long>(), fixture.Create<string>(), authorId)
        };

        var userName = fixture.Create<string>();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(authorId).Returns(UserDataTestFactory.Create(userName));

        var repository = Substitute.For<IRecipeCollectionRepository>();
        repository.LoadRecipeEntriesByTagEntityIdAsync(Arg.Any<long>()).ReturnsForAnyArgs(entries);

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByTagIdAsync(fixture.Create<long>());

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessage =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].EntityId,-3} {entries[0].Title,-50} {userName,-50}{Environment.NewLine}" +
            $"{entries[1].EntityId,-3} {entries[1].Title,-50} {userName,-50}{Environment.NewLine}";
        result.Result.Should().HaveCount(1).And.Contain(Format.Code(expectedMessage));
    }

    [Fact]
    public async Task Given_repository_returning_collection_and_message_within_limits_when_filtering_recipes_by_tag_id_returns_expected_message()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(int.MaxValue);

        var fixture = new Fixture();
        RecipeRepositoryEntityData[] entries = fixture.CreateMany<RecipeRepositoryEntityData>(3).ToArray();

        IReadOnlyList<UserData> userData = GetUsers(fixture, entries.Length);
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(entries[0].AuthorId).Returns(userData[0]);
        userDataProvider.GetUserDataAsync(entries[1].AuthorId).Returns(userData[1]);
        userDataProvider.GetUserDataAsync(entries[2].AuthorId).Returns(userData[2]);

        var repository = Substitute.For<IRecipeCollectionRepository>();
        repository.LoadRecipeEntriesByTagEntityIdAsync(Arg.Any<long>()).ReturnsForAnyArgs(entries);

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByTagIdAsync(fixture.Create<long>());

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessage =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].EntityId,-3} {entries[0].Title,-50} {userData[0].Username,-50}{Environment.NewLine}" +
            $"{entries[1].EntityId,-3} {entries[1].Title,-50} {userData[1].Username,-50}{Environment.NewLine}" +
            $"{entries[2].EntityId,-3} {entries[2].Title,-50} {userData[2].Username,-50}{Environment.NewLine}";
        result.Result.Should().HaveCount(1).And.Contain(Format.Code(expectedMessage));
    }

    [Theory]
    [InlineData(328)] // Exactly intersects with 2 entries
    [InlineData(340)]
    public async Task Given_repository_returning_collection_and_message_exceeding_limits_when_filtering_recipes_by_tag_id_returns_expected_messages(
        int maxMessageLength)
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(maxMessageLength);

        var fixture = new Fixture();
        RecipeRepositoryEntityData[] entries = fixture.CreateMany<RecipeRepositoryEntityData>(3).ToArray();

        IReadOnlyList<UserData> userData = GetUsers(fixture, entries.Length);
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(entries[0].AuthorId).Returns(userData[0]);
        userDataProvider.GetUserDataAsync(entries[1].AuthorId).Returns(userData[1]);
        userDataProvider.GetUserDataAsync(entries[2].AuthorId).Returns(userData[2]);

        var repository = Substitute.For<IRecipeCollectionRepository>();
        repository.LoadRecipeEntriesByTagEntityIdAsync(Arg.Any<long>()).ReturnsForAnyArgs(entries);

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByTagIdAsync(fixture.Create<long>());

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessageOne =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].EntityId,-3} {entries[0].Title,-50} {userData[0].Username,-50}{Environment.NewLine}" +
            $"{entries[1].EntityId,-3} {entries[1].Title,-50} {userData[1].Username,-50}{Environment.NewLine}";

        string expectedMessageTwo =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[2].EntityId,-3} {entries[2].Title,-50} {userData[2].Username,-50}{Environment.NewLine}";

        result.Result.Should().BeEquivalentTo(new[]
        {
            Format.Code(expectedMessageOne),
            Format.Code(expectedMessageTwo)
        }, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task Given_repository_throwing_exception_when_listing_by_tag_id_then_logs_and_returns_result_with_error()
    {
        // Setup
        var fixture = new Fixture();
        var exceptionMessage = fixture.Create<string>();

        var repository = Substitute.For<IRecipeCollectionRepository>();
        var exception = new RepositoryDataLoadException(exceptionMessage);
        repository.LoadRecipeEntriesByTagEntityIdAsync(Arg.Any<long>()).ThrowsAsyncForAnyArgs(exception);

        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByTagIdAsync(fixture.Create<long>());

        // Assert
        result.HasError.Should().BeTrue();
        result.ErrorMessage.Should().Be(exceptionMessage);

        logger.Received(1).LogError(exception);
    }

    [Fact]
    public async Task Given_repository_returning_empty_collection_when_filtering_recipes_by_author_returns_expected_message()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var userDataProvider = Substitute.For<IUserDataProvider>();

        var repository = Substitute.For<IRecipeCollectionRepository>();
        repository.LoadRecipeEntriesByTagEntityIdAsync(Arg.Any<long>()).ReturnsForAnyArgs(Array.Empty<RecipeRepositoryEntityData>());

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        var user = Substitute.For<IUser>();

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByUserAsync(user);

        // Assert
        result.HasError.Should().BeFalse();

        result.Result.Should().HaveCount(1).And.Contain("No saved recipes are found.");
    }

    [Fact]
    public async Task Filtering_recipes_by_user_repository_receives_correct_id()
    {
        // Setup
        var fixture = new Fixture();

        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var userDataProvider = Substitute.For<IUserDataProvider>();

        var user = Substitute.For<IUser>();
        var idToFilter = fixture.Create<ulong>();
        user.Id.Returns(idToFilter);

        var repository = Substitute.For<IRecipeCollectionRepository>();
        repository.LoadRecipeEntriesByAuthorIdAsync(idToFilter).ReturnsForAnyArgs(Array.Empty<RecipeRepositoryEntityData>());

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        await controller.GetAllRecipesByUserAsync(user);

        // Assert
        await repository.Received(1).LoadRecipeEntriesByAuthorIdAsync(idToFilter);
    }

    [Fact]
    public async Task Given_repository_returning_collection_without_distinct_author_when_filtering_recipes_by_user_returns_expected_message()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(int.MaxValue);

        var fixture = new Fixture();
        var authorId = fixture.Create<ulong>();

        var entries = new[]
        {
            new RecipeRepositoryEntityData(fixture.Create<long>(), fixture.Create<string>(), authorId),
            new RecipeRepositoryEntityData(fixture.Create<long>(), fixture.Create<string>(), authorId)
        };

        var userName = fixture.Create<string>();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(authorId).Returns(UserDataTestFactory.Create(userName));

        var repository = Substitute.For<IRecipeCollectionRepository>();
        repository.LoadRecipeEntriesByAuthorIdAsync(Arg.Any<ulong>()).ReturnsForAnyArgs(entries);

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByUserAsync(Substitute.For<IUser>());

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessage =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].EntityId,-3} {entries[0].Title,-50} {userName,-50}{Environment.NewLine}" +
            $"{entries[1].EntityId,-3} {entries[1].Title,-50} {userName,-50}{Environment.NewLine}";
        result.Result.Should().HaveCount(1).And.Contain(Format.Code(expectedMessage));
    }

    [Fact]
    public async Task Given_repository_returning_collection_and_message_within_limits_when_filtering_recipes_by_user_returns_expected_message()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(int.MaxValue);

        var fixture = new Fixture();
        var user = Substitute.For<IUser>();

        RecipeRepositoryEntityData[] entries = fixture.CreateMany<RecipeRepositoryEntityData>(3).ToArray();

        IReadOnlyList<UserData> userData = GetUsers(fixture, entries.Length);
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(entries[0].AuthorId).Returns(userData[0]);
        userDataProvider.GetUserDataAsync(entries[1].AuthorId).Returns(userData[1]);
        userDataProvider.GetUserDataAsync(entries[2].AuthorId).Returns(userData[2]);

        var repository = Substitute.For<IRecipeCollectionRepository>();
        repository.LoadRecipeEntriesByAuthorIdAsync(Arg.Any<ulong>()).ReturnsForAnyArgs(entries);

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByUserAsync(user);

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessage =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].EntityId,-3} {entries[0].Title,-50} {userData[0].Username,-50}{Environment.NewLine}" +
            $"{entries[1].EntityId,-3} {entries[1].Title,-50} {userData[1].Username,-50}{Environment.NewLine}" +
            $"{entries[2].EntityId,-3} {entries[2].Title,-50} {userData[2].Username,-50}{Environment.NewLine}";
        result.Result.Should().HaveCount(1).And.Contain(Format.Code(expectedMessage));
    }

    [Theory]
    [InlineData(328)] // Exactly intersects with 2 entries
    [InlineData(340)]
    public async Task Given_repository_returning_collection_and_message_exceeding_limits_when_filtering_recipes_by_user_returns_expected_messages(
        int maxMessageLength)
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(maxMessageLength);

        var fixture = new Fixture();
        var user = Substitute.For<IUser>();

        RecipeRepositoryEntityData[] entries = fixture.CreateMany<RecipeRepositoryEntityData>(3).ToArray();

        IReadOnlyList<UserData> userData = GetUsers(fixture, entries.Length);
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(entries[0].AuthorId).Returns(userData[0]);
        userDataProvider.GetUserDataAsync(entries[1].AuthorId).Returns(userData[1]);
        userDataProvider.GetUserDataAsync(entries[2].AuthorId).Returns(userData[2]);

        var repository = Substitute.For<IRecipeCollectionRepository>();
        repository.LoadRecipeEntriesByAuthorIdAsync(Arg.Any<ulong>()).ReturnsForAnyArgs(entries);

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByUserAsync(user);

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessageOne =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].EntityId,-3} {entries[0].Title,-50} {userData[0].Username,-50}{Environment.NewLine}" +
            $"{entries[1].EntityId,-3} {entries[1].Title,-50} {userData[1].Username,-50}{Environment.NewLine}";

        string expectedMessageTwo =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[2].EntityId,-3} {entries[2].Title,-50} {userData[2].Username,-50}{Environment.NewLine}";

        result.Result.Should().BeEquivalentTo(new[]
        {
            Format.Code(expectedMessageOne),
            Format.Code(expectedMessageTwo)
        }, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task Given_repository_throwing_exception_when_listing_by_author_id_then_logs_and_returns_result_with_error()
    {
        // Setup
        var fixture = new Fixture();
        var user = Substitute.For<IUser>();

        var exceptionMessage = fixture.Create<string>();

        var repository = Substitute.For<IRecipeCollectionRepository>();
        var exception = new RepositoryDataLoadException(exceptionMessage);
        repository.LoadRecipeEntriesByAuthorIdAsync(Arg.Any<ulong>()).ThrowsAsyncForAnyArgs(exception);

        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByUserAsync(user);

        // Assert
        result.HasError.Should().BeTrue();
        result.ErrorMessage.Should().Be(exceptionMessage);

        logger.Received(1).LogError(exception);
    }

    private static IReadOnlyList<UserData> GetUsers(Fixture fixture, int nrOfUsers)
    {
        var users = new UserData[nrOfUsers];
        for (var i = 0; i < nrOfUsers; i++)
        {
            users[i] = UserDataTestFactory.Create(fixture.Create<string>());
        }

        return users;
    }
}