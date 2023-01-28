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
using Discord.Common;
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
        var repository = Substitute.For<IRecipeDataEntryCollectionRepository>();
        var logger = Substitute.For<ILoggingService>();

        // Call
        var controller = new RecipeEntriesController(limitProvider, repository, logger);

        // Assert
        controller.Should().BeAssignableTo<IRecipeEntriesController>();
    }

    [Fact]
    public async Task Given_repository_returning_empty_collection_when_listing_all_recipes_returns_expected_message()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var logger = Substitute.For<ILoggingService>();

        var repository = Substitute.For<IRecipeDataEntryCollectionRepository>();
        repository.LoadRecipeEntriesAsync().ReturnsForAnyArgs(Array.Empty<RecipeEntryData>());

        var controller = new RecipeEntriesController(limitProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.ListAllRecipesAsync();

        // Assert
        result.HasError.Should().BeFalse();

        result.Result.Should().HaveCount(1).And.Contain("No saved recipes are found.");
    }

    [Fact]
    public async Task Given_repository_returning_collection_and_message_within_limits_when_listing_all_recipes_returns_expected_message()
    {
        // Setup
        var logger = Substitute.For<ILoggingService>();

        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(int.MaxValue);

        var fixture = new Fixture();
        RecipeEntryData[] entries = fixture.CreateMany<RecipeEntryData>(3).ToArray();

        var repository = Substitute.For<IRecipeDataEntryCollectionRepository>();
        repository.LoadRecipeEntriesAsync().ReturnsForAnyArgs(entries);

        var controller = new RecipeEntriesController(limitProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.ListAllRecipesAsync();

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessage =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].Id,-3} {entries[0].Title,-50} {entries[0].AuthorName,-50}{Environment.NewLine}" +
            $"{entries[1].Id,-3} {entries[1].Title,-50} {entries[1].AuthorName,-50}{Environment.NewLine}" +
            $"{entries[2].Id,-3} {entries[2].Title,-50} {entries[2].AuthorName,-50}{Environment.NewLine}";
        result.Result.Should().HaveCount(1).And.Contain(Format.Code(expectedMessage));
    }

    [Theory]
    [InlineData(328)] // Exactly intersects with 2 entries
    [InlineData(340)]
    public async Task Given_repository_returning_collection_and_message_exceeding_limits_when_listing_all_recipes_returns_expected_messages(
        int maxMessageLength)
    {
        // Setup
        var logger = Substitute.For<ILoggingService>();

        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(maxMessageLength);

        var fixture = new Fixture();
        RecipeEntryData[] entries = fixture.CreateMany<RecipeEntryData>(3).ToArray();

        var repository = Substitute.For<IRecipeDataEntryCollectionRepository>();
        repository.LoadRecipeEntriesAsync().ReturnsForAnyArgs(entries);

        var controller = new RecipeEntriesController(limitProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.ListAllRecipesAsync();

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessageOne =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].Id,-3} {entries[0].Title,-50} {entries[0].AuthorName,-50}{Environment.NewLine}" +
            $"{entries[1].Id,-3} {entries[1].Title,-50} {entries[1].AuthorName,-50}{Environment.NewLine}";

        string expectedMessageTwo =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[2].Id,-3} {entries[2].Title,-50} {entries[2].AuthorName,-50}{Environment.NewLine}";

        result.Result.Should().BeEquivalentTo(new[]
        {
            Format.Code(expectedMessageOne), 
            Format.Code(expectedMessageTwo)
        }, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task Given_repository_returning_empty_collection_when_listing_recipes_with_category_filter_returns_expected_message()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var logger = Substitute.For<ILoggingService>();

        var repository = Substitute.For<IRecipeDataEntryCollectionRepository>();
        repository.LoadRecipeEntriesAsync(Arg.Any<RecipeCategory>()).ReturnsForAnyArgs(Array.Empty<RecipeEntryData>());

        var controller = new RecipeEntriesController(limitProvider, repository, logger);

        var fixture = new Fixture();

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.ListAllRecipesAsync(fixture.Create<DiscordRecipeCategory>());

        // Assert
        result.HasError.Should().BeFalse();

        result.Result.Should().HaveCount(1).And.Contain("No saved recipes are found based on the given category.");
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
    public async Task Listing_recipes_with_category_filter_repository_receives_correct_category_filter(DiscordRecipeCategory category)
    {
        // Setup
        var logger = Substitute.For<ILoggingService>();
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();

        var repository = Substitute.For<IRecipeDataEntryCollectionRepository>();
        repository.LoadRecipeEntriesAsync(Arg.Any<RecipeCategory>()).ReturnsForAnyArgs(Array.Empty<RecipeEntryData>());

        var controller = new RecipeEntriesController(limitProvider, repository, logger);

        // Call
        await controller.ListAllRecipesAsync(category);

        // Assert
        await repository.Received(1).LoadRecipeEntriesAsync(DiscordRecipeCategoryTestHelper.RecipeCategoryMapping[category]);
    }

    [Fact]
    public async Task Given_repository_returning_collection_and_message_within_limits_when_listing_recipes_with_category_filter_returns_expected_message()
    {
        // Setup
        var logger = Substitute.For<ILoggingService>();

        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(int.MaxValue);

        var fixture = new Fixture();
        RecipeEntryData[] entries = fixture.CreateMany<RecipeEntryData>(3).ToArray();

        var repository = Substitute.For<IRecipeDataEntryCollectionRepository>();
        repository.LoadRecipeEntriesAsync(Arg.Any<RecipeCategory>()).ReturnsForAnyArgs(entries);

        var controller = new RecipeEntriesController(limitProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.ListAllRecipesAsync(fixture.Create<DiscordRecipeCategory>());

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessage =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].Id,-3} {entries[0].Title,-50} {entries[0].AuthorName,-50}{Environment.NewLine}" +
            $"{entries[1].Id,-3} {entries[1].Title,-50} {entries[1].AuthorName,-50}{Environment.NewLine}" +
            $"{entries[2].Id,-3} {entries[2].Title,-50} {entries[2].AuthorName,-50}{Environment.NewLine}";
        result.Result.Should().HaveCount(1).And.Contain(Format.Code(expectedMessage));
    }

    [Theory]
    [InlineData(328)] // Exactly intersects with 2 entries
    [InlineData(340)]
    public async Task Given_repository_returning_collection_and_message_exceeding_limits_when_listing_recipes_with_categroy_filter_returns_expected_messages(
        int maxMessageLength)
    {
        // Setup
        var logger = Substitute.For<ILoggingService>();

        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(maxMessageLength);

        var fixture = new Fixture();
        RecipeEntryData[] entries = fixture.CreateMany<RecipeEntryData>(3).ToArray();

        var repository = Substitute.For<IRecipeDataEntryCollectionRepository>();
        repository.LoadRecipeEntriesAsync(Arg.Any<RecipeCategory>()).ReturnsForAnyArgs(entries);

        var controller = new RecipeEntriesController(limitProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.ListAllRecipesAsync(fixture.Create<DiscordRecipeCategory>());

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessageOne =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].Id,-3} {entries[0].Title,-50} {entries[0].AuthorName,-50}{Environment.NewLine}" +
            $"{entries[1].Id,-3} {entries[1].Title,-50} {entries[1].AuthorName,-50}{Environment.NewLine}";

        string expectedMessageTwo =
            $"{"Id",-3} {"Title",-50} {"Author",-50} {Environment.NewLine}" +
            $"{entries[2].Id,-3} {entries[2].Title,-50} {entries[2].AuthorName,-50}{Environment.NewLine}";

        result.Result.Should().BeEquivalentTo(new[]
        {
            Format.Code(expectedMessageOne),
            Format.Code(expectedMessageTwo)
        }, options => options.WithStrictOrdering());
    }
}