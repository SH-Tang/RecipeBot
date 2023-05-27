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
using Common.Utils;
using Discord;
using Discord.Common.Providers;
using RecipeBot.Discord.Controllers;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Factories;
using RecipeBot.Domain.Repositories;
using RecipeBot.Domain.Repositories.Data;
using RecipeBot.Properties;
using RecipeBot.Services;
using Discord.Common.Services;

namespace RecipeBot.Controllers;

/// <summary>
/// A concrete implementation of <see cref="IAuthorController"/>.
/// </summary>
public class AuthorController : IAuthorController
{
    private readonly ILoggingService logger;
    private readonly IAuthorRepository repository;
    private readonly IUserDataProvider userDataProvider;

    /// <summary>
    /// Creates a new instance of <see cref="AuthorController"/>.
    /// </summary>
    /// <param name="userDataProvider">The provider to retrieve user data with.</param>
    /// <param name="repository">The repository to handle with the persistence of authors.</param>
    /// <param name="logger">The logger to log with.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    public AuthorController(IUserDataProvider userDataProvider,
                            IAuthorRepository repository,
                            ILoggingService logger)
    {
        userDataProvider.IsNotNull(nameof(userDataProvider));
        repository.IsNotNull(nameof(repository));
        logger.IsNotNull(nameof(logger));

        this.userDataProvider = userDataProvider;
        this.repository = repository;
        this.logger = logger;
    }

    public async Task<ControllerResult<string>> DeleteAuthorAsync(IUser user)
    {
        user.IsNotNull(nameof(user));

        try
        {
            ulong authorId = user.Id;
            UserData userData = await userDataProvider.GetUserDataAsync(authorId);
            await repository.DeleteEntityAsync(authorId);

            return ControllerResult<string>.CreateControllerResultWithValidResult(
                string.Format(Resources.AuthorController_DeleteAuthor_All_data_of_Author_0_was_successfully_deleted, userData.Username));
        }
        catch (RepositoryDataDeleteException e)
        {
            return await HandleException<string>(e);
        }
    }
    private async Task<ControllerResult<TResult>> HandleException<TResult>(Exception e) where TResult : class
    {
        await logger.LogError(e);
        return ControllerResult<TResult>.CreateControllerResultWithError(e.Message);
    }
}