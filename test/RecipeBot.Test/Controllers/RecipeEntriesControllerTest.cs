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
using Discord.Common.TestUtils;
using FluentAssertions;
using NSubstitute;
using RecipeBot.Controllers;
using RecipeBot.Discord.Controllers;
using RecipeBot.Discord.Data;
using RecipeBot.Domain.Data;
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
        var repository = Substitute.For<IRecipeDataEntryCollectionRepository>();

        // Call
        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository);

        // Assert
        controller.Should().BeAssignableTo<IRecipeEntriesController>();
    }

    [Fact]
    public async Task Given_repository_returning_empty_collection_when_listing_all_recipes_returns_expected_message()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var userDataProvider = Substitute.For<IUserDataProvider>();

        var repository = Substitute.For<IRecipeDataEntryCollectionRepository>();
        repository.LoadRecipeEntriesAsync().ReturnsForAnyArgs(Array.Empty<RecipeEntryData>());

        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesAsync();

        // Assert
        result.HasError.Should().BeFalse();

        result.Result.Should().HaveCount(1).And.Contain("No saved recipes are found.");
    }

    [Fact]
    public async Task Given_repository_returning_collection_and_message_within_limits_when_listing_all_recipes_returns_expected_message()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(int.MaxValue);

        var fixture = new Fixture();
        RecipeEntryData[] entries = fixture.CreateMany<RecipeEntryData>(3).ToArray();

        var repository = Substitute.For<IRecipeDataEntryCollectionRepository>();
        repository.LoadRecipeEntriesAsync().ReturnsForAnyArgs(entries);

        IReadOnlyList<UserData> userData = GetUsers(fixture, entries.Length);
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(entries[0].AuthorId).Returns(userData[0]);
        userDataProvider.GetUserDataAsync(entries[1].AuthorId).Returns(userData[1]);
        userDataProvider.GetUserDataAsync(entries[2].AuthorId).Returns(userData[2]);

        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesAsync();

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessage =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].Id,-3} {entries[0].Title,-50} {userData[0].Username,-50}{Environment.NewLine}" +
            $"{entries[1].Id,-3} {entries[1].Title,-50} {userData[1].Username,-50}{Environment.NewLine}" +
            $"{entries[2].Id,-3} {entries[2].Title,-50} {userData[2].Username,-50}{Environment.NewLine}";
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
        RecipeEntryData[] entries = fixture.CreateMany<RecipeEntryData>(3).ToArray();

        IReadOnlyList<UserData> userData = GetUsers(fixture, entries.Length);
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(entries[0].AuthorId).Returns(userData[0]);
        userDataProvider.GetUserDataAsync(entries[1].AuthorId).Returns(userData[1]);
        userDataProvider.GetUserDataAsync(entries[2].AuthorId).Returns(userData[2]);

        var repository = Substitute.For<IRecipeDataEntryCollectionRepository>();
        repository.LoadRecipeEntriesAsync().ReturnsForAnyArgs(entries);

        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesAsync();

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessageOne =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].Id,-3} {entries[0].Title,-50} {userData[0].Username,-50}{Environment.NewLine}" +
            $"{entries[1].Id,-3} {entries[1].Title,-50} {userData[1].Username,-50}{Environment.NewLine}";

        string expectedMessageTwo =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[2].Id,-3} {entries[2].Title,-50} {userData[2].Username,-50}{Environment.NewLine}";

        result.Result.Should().BeEquivalentTo(new[]
        {
            Format.Code(expectedMessageOne),
            Format.Code(expectedMessageTwo)
        }, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task Given_repository_returning_empty_collection_when_filtering_recipes_by_category_returns_expected_message()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var userDataProvider = Substitute.For<IUserDataProvider>();

        var repository = Substitute.For<IRecipeDataEntryCollectionRepository>();
        repository.LoadRecipeEntriesByCategoryAsync(Arg.Any<RecipeCategory>()).ReturnsForAnyArgs(Array.Empty<RecipeEntryData>());

        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository);

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

        var repository = Substitute.For<IRecipeDataEntryCollectionRepository>();
        repository.LoadRecipeEntriesByCategoryAsync(Arg.Any<RecipeCategory>()).ReturnsForAnyArgs(Array.Empty<RecipeEntryData>());

        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository);

        // Call
        await controller.GetAllRecipesByCategoryAsync(category);

        // Assert
        await repository.Received(1).LoadRecipeEntriesByCategoryAsync(DiscordRecipeCategoryTestHelper.RecipeCategoryMapping[category]);
    }

    [Fact]
    public async Task Given_repository_returning_collection_and_message_within_limits_when_filtering_recipes_by_category_returns_expected_message()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(int.MaxValue);

        var fixture = new Fixture();
        RecipeEntryData[] entries = fixture.CreateMany<RecipeEntryData>(3).ToArray();

        IReadOnlyList<UserData> userData = GetUsers(fixture, entries.Length);
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(entries[0].AuthorId).Returns(userData[0]);
        userDataProvider.GetUserDataAsync(entries[1].AuthorId).Returns(userData[1]);
        userDataProvider.GetUserDataAsync(entries[2].AuthorId).Returns(userData[2]);

        var repository = Substitute.For<IRecipeDataEntryCollectionRepository>();
        repository.LoadRecipeEntriesByCategoryAsync(Arg.Any<RecipeCategory>()).ReturnsForAnyArgs(entries);

        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByCategoryAsync(fixture.Create<DiscordRecipeCategory>());

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessage =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].Id,-3} {entries[0].Title,-50} {userData[0].Username,-50}{Environment.NewLine}" +
            $"{entries[1].Id,-3} {entries[1].Title,-50} {userData[1].Username,-50}{Environment.NewLine}" +
            $"{entries[2].Id,-3} {entries[2].Title,-50} {userData[2].Username,-50}{Environment.NewLine}";
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
        RecipeEntryData[] entries = fixture.CreateMany<RecipeEntryData>(3).ToArray();

        IReadOnlyList<UserData> userData = GetUsers(fixture, entries.Length);
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(entries[0].AuthorId).Returns(userData[0]);
        userDataProvider.GetUserDataAsync(entries[1].AuthorId).Returns(userData[1]);
        userDataProvider.GetUserDataAsync(entries[2].AuthorId).Returns(userData[2]);

        var repository = Substitute.For<IRecipeDataEntryCollectionRepository>();
        repository.LoadRecipeEntriesByCategoryAsync(Arg.Any<RecipeCategory>()).ReturnsForAnyArgs(entries);

        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByCategoryAsync(fixture.Create<DiscordRecipeCategory>());

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessageOne =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].Id,-3} {entries[0].Title,-50} {userData[0].Username,-50}{Environment.NewLine}" +
            $"{entries[1].Id,-3} {entries[1].Title,-50} {userData[1].Username,-50}{Environment.NewLine}";

        string expectedMessageTwo =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[2].Id,-3} {entries[2].Title,-50} {userData[2].Username,-50}{Environment.NewLine}";

        result.Result.Should().BeEquivalentTo(new[]
        {
            Format.Code(expectedMessageOne),
            Format.Code(expectedMessageTwo)
        }, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task Given_repository_returning_empty_collection_when_filtering_recipes_by_tag_returns_expected_message()
    {
        // Setup
        var fixture = new Fixture();
        var tagToFilter = fixture.Create<string>();

        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var userDataProvider = Substitute.For<IUserDataProvider>();

        var repository = Substitute.For<IRecipeDataEntryCollectionRepository>();
        repository.LoadRecipeEntriesByTagAsync(Arg.Any<string>()).ReturnsForAnyArgs(Array.Empty<RecipeEntryData>());

        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository);

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

        var repository = Substitute.For<IRecipeDataEntryCollectionRepository>();
        repository.LoadRecipeEntriesByTagAsync(expectedTagArgument).ReturnsForAnyArgs(Array.Empty<RecipeEntryData>());

        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository);

        // Call
        await controller.GetAllRecipesByTagAsync(tag);

        // Assert
        await repository.Received(1).LoadRecipeEntriesByTagAsync(expectedTagArgument);
    }

    [Fact]
    public async Task Given_repository_returning_collection_and_message_within_limits_when_filtering_recipes_by_tag_returns_expected_message()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(int.MaxValue);

        var fixture = new Fixture();
        RecipeEntryData[] entries = fixture.CreateMany<RecipeEntryData>(3).ToArray();

        IReadOnlyList<UserData> userData = GetUsers(fixture, entries.Length);
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(entries[0].AuthorId).Returns(userData[0]);
        userDataProvider.GetUserDataAsync(entries[1].AuthorId).Returns(userData[1]);
        userDataProvider.GetUserDataAsync(entries[2].AuthorId).Returns(userData[2]);

        var repository = Substitute.For<IRecipeDataEntryCollectionRepository>();
        repository.LoadRecipeEntriesByTagAsync(Arg.Any<string>()).ReturnsForAnyArgs(entries);

        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByTagAsync(fixture.Create<string>());

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessage =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].Id,-3} {entries[0].Title,-50} {userData[0].Username,-50}{Environment.NewLine}" +
            $"{entries[1].Id,-3} {entries[1].Title,-50} {userData[1].Username,-50}{Environment.NewLine}" +
            $"{entries[2].Id,-3} {entries[2].Title,-50} {userData[2].Username,-50}{Environment.NewLine}";
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
        RecipeEntryData[] entries = fixture.CreateMany<RecipeEntryData>(3).ToArray();

        IReadOnlyList<UserData> userData = GetUsers(fixture, entries.Length);
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(entries[0].AuthorId).Returns(userData[0]);
        userDataProvider.GetUserDataAsync(entries[1].AuthorId).Returns(userData[1]);
        userDataProvider.GetUserDataAsync(entries[2].AuthorId).Returns(userData[2]);

        var repository = Substitute.For<IRecipeDataEntryCollectionRepository>();
        repository.LoadRecipeEntriesByTagAsync(Arg.Any<string>()).ReturnsForAnyArgs(entries);

        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByTagAsync(fixture.Create<string>());

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessageOne =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].Id,-3} {entries[0].Title,-50} {userData[0].Username,-50}{Environment.NewLine}" +
            $"{entries[1].Id,-3} {entries[1].Title,-50} {userData[1].Username,-50}{Environment.NewLine}";

        string expectedMessageTwo =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[2].Id,-3} {entries[2].Title,-50} {userData[2].Username,-50}{Environment.NewLine}";

        result.Result.Should().BeEquivalentTo(new[]
        {
            Format.Code(expectedMessageOne),
            Format.Code(expectedMessageTwo)
        }, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task Given_repository_returning_empty_collection_when_filtering_recipes_by_tag_id_returns_expected_message()
    {
        // Setup
        var fixture = new Fixture();
        var idToFilter = fixture.Create<long>();

        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var userDataProvider = Substitute.For<IUserDataProvider>();

        var repository = Substitute.For<IRecipeDataEntryCollectionRepository>();
        repository.LoadRecipeEntriesByTagIdAsync(Arg.Any<long>()).ReturnsForAnyArgs(Array.Empty<RecipeEntryData>());

        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository);

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

        var repository = Substitute.For<IRecipeDataEntryCollectionRepository>();
        repository.LoadRecipeEntriesByTagIdAsync(idToFilter).ReturnsForAnyArgs(Array.Empty<RecipeEntryData>());

        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository);

        // Call
        await controller.GetAllRecipesByTagIdAsync(idToFilter);

        // Assert
        await repository.Received(1).LoadRecipeEntriesByTagIdAsync(idToFilter);
    }

    [Fact]
    public async Task Given_repository_returning_collection_and_message_within_limits_when_filtering_recipes_by_tag_id_returns_expected_message()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(int.MaxValue);

        var fixture = new Fixture();
        RecipeEntryData[] entries = fixture.CreateMany<RecipeEntryData>(3).ToArray();

        IReadOnlyList<UserData> userData = GetUsers(fixture, entries.Length);
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(entries[0].AuthorId).Returns(userData[0]);
        userDataProvider.GetUserDataAsync(entries[1].AuthorId).Returns(userData[1]);
        userDataProvider.GetUserDataAsync(entries[2].AuthorId).Returns(userData[2]);

        var repository = Substitute.For<IRecipeDataEntryCollectionRepository>();
        repository.LoadRecipeEntriesByTagIdAsync(Arg.Any<long>()).ReturnsForAnyArgs(entries);

        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByTagIdAsync(fixture.Create<long>());

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessage =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].Id,-3} {entries[0].Title,-50} {userData[0].Username,-50}{Environment.NewLine}" +
            $"{entries[1].Id,-3} {entries[1].Title,-50} {userData[1].Username,-50}{Environment.NewLine}" +
            $"{entries[2].Id,-3} {entries[2].Title,-50} {userData[2].Username,-50}{Environment.NewLine}";
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
        RecipeEntryData[] entries = fixture.CreateMany<RecipeEntryData>(3).ToArray();

        IReadOnlyList<UserData> userData = GetUsers(fixture, entries.Length);
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(entries[0].AuthorId).Returns(userData[0]);
        userDataProvider.GetUserDataAsync(entries[1].AuthorId).Returns(userData[1]);
        userDataProvider.GetUserDataAsync(entries[2].AuthorId).Returns(userData[2]);

        var repository = Substitute.For<IRecipeDataEntryCollectionRepository>();
        repository.LoadRecipeEntriesByTagIdAsync(Arg.Any<long>()).ReturnsForAnyArgs(entries);

        var controller = new RecipeEntriesController(limitProvider, userDataProvider, repository);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllRecipesByTagIdAsync(fixture.Create<long>());

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessageOne =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].Id,-3} {entries[0].Title,-50} {userData[0].Username,-50}{Environment.NewLine}" +
            $"{entries[1].Id,-3} {entries[1].Title,-50} {userData[1].Username,-50}{Environment.NewLine}";

        string expectedMessageTwo =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[2].Id,-3} {entries[2].Title,-50} {userData[2].Username,-50}{Environment.NewLine}";

        result.Result.Should().BeEquivalentTo(new[]
        {
            Format.Code(expectedMessageOne),
            Format.Code(expectedMessageTwo)
        }, options => options.WithStrictOrdering());
    }

    private IReadOnlyList<UserData> GetUsers(Fixture fixture, int nrOfUsers)
    {
        var users = new UserData[nrOfUsers];
        for (var i = 0; i < nrOfUsers; i++)
        {
            users[i] = UserDataTestFactory.Create(fixture.Create<string>());
        }

        return users;
    }
}