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
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Repositories;
using RecipeBot.Domain.Repositories.Data;
using Xunit;

namespace RecipeBot.Test.Controllers;

public class AuthorControllerTest
{
    [Fact]
    public void Controller_is_author_controller()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        var repository = Substitute.For<IAuthorRepository>();
        var logger = Substitute.For<ILoggingService>();

        // Call
        var controller = new AuthorController(limitProvider, userDataProvider, repository, logger);

        // Assert
        controller.Should().BeAssignableTo<IAuthorController>();
    }

    [Fact]
    public async Task Deleting_author_and_delete_successful_returns_result()
    {
        // Setup
        var fixture = new Fixture();
        var authorIdToDelete = fixture.Create<ulong>();

        var user = Substitute.For<IUser>();
        user.Id.Returns(authorIdToDelete);

        UserData userData = UserDataTestFactory.CreateFullyConfigured();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(authorIdToDelete).Returns(userData);

        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var repository = Substitute.For<IAuthorRepository>();
        var logger = Substitute.For<ILoggingService>();
        var controller = new AuthorController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<string> result = await controller.DeleteAuthorAsync(user);

        // Assert
        result.HasError.Should().BeFalse();

        result.Result.Should().Be($"All data of '{userData.Username}' was successfully deleted.");
    }

    [Fact]
    public async Task Deleting_author_and_delete_unsuccessful_logs_and_returns_result_with_error()
    {
        // Setup
        var fixture = new Fixture();
        var exceptionMessage = fixture.Create<string>();

        var repository = Substitute.For<IAuthorRepository>();
        var exception = new RepositoryDataDeleteException(exceptionMessage);
        repository.DeleteAuthorAsync(Arg.Any<ulong>()).ThrowsAsyncForAnyArgs(exception);

        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        var logger = Substitute.For<ILoggingService>();
        var controller = new AuthorController(limitProvider, userDataProvider, repository, logger);

        var user = Substitute.For<IUser>();

        // Call
        ControllerResult<string> result = await controller.DeleteAuthorAsync(user);

        // Assert
        result.HasError.Should().BeTrue();
        result.ErrorMessage.Should().Be(exceptionMessage);

        await logger.Received(1).LogErrorAsync(exception);
    }

    [Fact]
    public async Task Given_repository_returning_empty_collection_when_listing_all_authors_returns_expected_message()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(int.MaxValue);

        var repository = Substitute.For<IAuthorRepository>();
        repository.LoadAuthorsAsync().ReturnsForAnyArgs(Array.Empty<AuthorRepositoryEntityData>());

        var logger = Substitute.For<ILoggingService>();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        var controller = new AuthorController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllAuthorsAsync();

        // Assert
        result.HasError.Should().BeFalse();

        result.Result.Should().HaveCount(1).And.Contain("No saved authors are found.");
    }

    [Fact]
    public async Task Given_repository_returning_collection_and_message_within_limits_when_listing_authors_returns_expected_messages()
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(int.MaxValue);

        var fixture = new Fixture();
        AuthorRepositoryEntityData[] entries = fixture.CreateMany<AuthorRepositoryEntityData>(3).ToArray();

        var repository = Substitute.For<IAuthorRepository>();
        repository.LoadAuthorsAsync().ReturnsForAnyArgs(entries);

        IReadOnlyList<UserData> userData = GetUsers(fixture, entries.Length);
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(entries[0].AuthorId).Returns(userData[0]);
        userDataProvider.GetUserDataAsync(entries[1].AuthorId).Returns(userData[1]);
        userDataProvider.GetUserDataAsync(entries[2].AuthorId).Returns(userData[2]);

        var logger = Substitute.For<ILoggingService>();
        var controller = new AuthorController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllAuthorsAsync();

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessage =
            $"{"Id",-3} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].EntityId,-3} {userData[0].Username,-50}{Environment.NewLine}" +
            $"{entries[1].EntityId,-3} {userData[1].Username,-50}{Environment.NewLine}" +
            $"{entries[2].EntityId,-3} {userData[2].Username,-50}{Environment.NewLine}";
        result.Result.Should().HaveCount(1).And.Contain(Format.Code(expectedMessage));
    }

    [Theory]
    [InlineData(175)] // Exactly intersects with 2 entries
    [InlineData(185)]
    public async Task Given_repository_returning_collection_and_message_exceeding_limits_when_listing_all_authors_returns_expected_messages(
        int maxMessageLength)
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(maxMessageLength);

        var fixture = new Fixture();
        AuthorRepositoryEntityData[] entries = fixture.CreateMany<AuthorRepositoryEntityData>(3).ToArray();

        var repository = Substitute.For<IAuthorRepository>();
        repository.LoadAuthorsAsync().ReturnsForAnyArgs(entries);

        IReadOnlyList<UserData> userData = GetUsers(fixture, entries.Length);
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(entries[0].AuthorId).Returns(userData[0]);
        userDataProvider.GetUserDataAsync(entries[1].AuthorId).Returns(userData[1]);
        userDataProvider.GetUserDataAsync(entries[2].AuthorId).Returns(userData[2]);

        var logger = Substitute.For<ILoggingService>();
        var controller = new AuthorController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllAuthorsAsync();

        // Assert
        result.HasError.Should().BeFalse();

        string expectedMessageOne =
            $"{"Id",-3} {"Author",-50} {Environment.NewLine}" +
            $"{entries[0].EntityId,-3} {userData[0].Username,-50}{Environment.NewLine}" +
            $"{entries[1].EntityId,-3} {userData[1].Username,-50}{Environment.NewLine}";

        string expectedMessageTwo =
            $"{"Id",-3} {"Author",-50} {Environment.NewLine}" +
            $"{entries[2].EntityId,-3} {userData[2].Username,-50}{Environment.NewLine}";

        result.Result.Should().BeEquivalentTo(new[]
        {
            Format.Code(expectedMessageOne),
            Format.Code(expectedMessageTwo)
        }, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task Given_repository_throwing_exception_when_listing_authors_logs_and_returns_result_with_error()
    {
        // Setup
        var fixture = new Fixture();
        var exceptionMessage = fixture.Create<string>();

        var repository = Substitute.For<IAuthorRepository>();
        var exception = new RepositoryDataLoadException(exceptionMessage);
        repository.LoadAuthorsAsync().ThrowsAsyncForAnyArgs(exception);

        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        var logger = Substitute.For<ILoggingService>();
        var controller = new AuthorController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<IReadOnlyList<string>> result = await controller.GetAllAuthorsAsync();

        // Assert
        result.HasError.Should().BeTrue();
        result.ErrorMessage.Should().Be(exceptionMessage);

        await logger.Received(1).LogErrorAsync(exception);
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