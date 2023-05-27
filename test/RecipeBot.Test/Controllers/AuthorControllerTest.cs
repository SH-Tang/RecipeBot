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
using Xunit;

namespace RecipeBot.Test.Controllers;

public class AuthorControllerTest
{
    [Fact]
    public void Controller_is_author_controller()
    {
        // Setup
        var userDataProvider = Substitute.For<IUserDataProvider>();
        var repository = Substitute.For<IAuthorRepository>();
        var logger = Substitute.For<ILoggingService>();

        // Call
        var controller = new AuthorController(userDataProvider, repository, logger);

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

        var logger = Substitute.For<ILoggingService>();
        var repository = Substitute.For<IAuthorRepository>();
        var controller = new AuthorController(userDataProvider, repository, logger);

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
        repository.DeleteEntityAsync(Arg.Any<ulong>()).ThrowsAsyncForAnyArgs(exception);

        var userDataProvider = Substitute.For<IUserDataProvider>();
        var logger = Substitute.For<ILoggingService>();
        var controller = new AuthorController(userDataProvider, repository, logger);

        var user = Substitute.For<IUser>();

        // Call
        ControllerResult<string> result = await controller.DeleteAuthorAsync(user);

        // Assert
        result.HasError.Should().BeTrue();
        result.ErrorMessage.Should().Be(exceptionMessage);

        await logger.Received(1).LogErrorAsync(exception);
    }
}