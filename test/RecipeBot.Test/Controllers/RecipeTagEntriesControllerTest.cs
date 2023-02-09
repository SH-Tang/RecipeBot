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
using FluentAssertions;
using NSubstitute;
using RecipeBot.Controllers;
using RecipeBot.Discord.Controllers;
using RecipeBot.Domain.Repositories;
using RecipeBot.Domain.Repositories.Data;
using Xunit;

namespace RecipeBot.Test.Controllers;

public class RecipeTagEntriesControllerTest
{
    [Fact]
    public void Controller_is_recipe_tag_entries_controller()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var repository = Substitute.For<IRecipeTagEntryDataRepository>();

        // Call
        var controller = new RecipeTagEntriesController(limitProvider, repository);

        // Assert
        controller.Should().BeAssignableTo<IRecipeTagEntriesController>();
    }

    [Fact]
    public async Task Given_repository_returning_empty_collection_when_listing_all_tags_returns_expected_message()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();

        var repository = Substitute.For<IRecipeTagEntryDataRepository>();
        repository.LoadRecipeTagEntriesAsync().ReturnsForAnyArgs(Array.Empty<RecipeTagEntryData>());

        var controller = new RecipeTagEntriesController(limitProvider, repository);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.ListAllTagsAsync();

        // Assert
        result.HasError.Should().BeFalse();

        result.Result.Should().HaveCount(1).And.Contain("No saved tags are found.");
    }

    [Fact]
    public async Task Given_repository_returning_collection_and_message_within_limits_when_listing_all_tags_returns_expected_message()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(int.MaxValue);

        var fixture = new Fixture();
        RecipeTagEntryData[] entries = fixture.CreateMany<RecipeTagEntryData>(3).ToArray();

        var repository = Substitute.For<IRecipeTagEntryDataRepository>();
        repository.LoadRecipeTagEntriesAsync().ReturnsForAnyArgs(entries);

        var controller = new RecipeTagEntriesController(limitProvider, repository);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.ListAllTagsAsync();

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessage =
            $"{"Id",-3} {"Tag",-50} {Environment.NewLine}" +
            $"{entries[0].Id,-3} {entries[0].Tag,-50}{Environment.NewLine}" +
            $"{entries[1].Id,-3} {entries[1].Tag,-50}{Environment.NewLine}" +
            $"{entries[2].Id,-3} {entries[2].Tag,-50}{Environment.NewLine}";
        result.Result.Should().HaveCount(1).And.Contain(Format.Code(expectedMessage));
    }

    [Theory]
    [InlineData(177)] // Exactly intersects with 2 entries
    [InlineData(185)]
    public async Task Given_repository_returning_collection_and_message_exceeding_limits_when_listing_all_tags_returns_expected_messages(
        int maxMessageLength)
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(maxMessageLength);

        var fixture = new Fixture();
        RecipeTagEntryData[] entries = fixture.CreateMany<RecipeTagEntryData>(3).ToArray();

        var repository = Substitute.For<IRecipeTagEntryDataRepository>();
        repository.LoadRecipeTagEntriesAsync().ReturnsForAnyArgs(entries);

        var controller = new RecipeTagEntriesController(limitProvider, repository);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.ListAllTagsAsync();

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessageOne =
            $"{"Id",-3} {"Tag",-50} {Environment.NewLine}" +
            $"{entries[0].Id,-3} {entries[0].Tag,-50}{Environment.NewLine}" +
            $"{entries[1].Id,-3} {entries[1].Tag,-50}{Environment.NewLine}";

        string expectedMessageTwo =
            $"{"Id",-3} {"Tag",-50} {Environment.NewLine}" +
            $"{entries[2].Id,-3} {entries[2].Tag,-50}{Environment.NewLine}";

        result.Result.Should().BeEquivalentTo(new[]
        {
            Format.Code(expectedMessageOne),
            Format.Code(expectedMessageTwo)
        }, options => options.WithStrictOrdering());
    }
}