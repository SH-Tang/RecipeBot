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
using Common.Utils;
using Discord;
using Discord.Common.Providers;
using Discord.Common.Services;
using RecipeBot.Discord.Controllers;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Repositories;
using RecipeBot.Domain.Repositories.Data;
using RecipeBot.Properties;
using RecipeBot.Services;

namespace RecipeBot.Controllers;

/// <summary>
/// A concrete implementation of <see cref="IAuthorController"/>.
/// </summary>
public class AuthorController : ControllerBase, IAuthorController
{
    private static readonly string header = $"{"Id",-3} {"Author",-50} ";

    private readonly DataEntryCollectionMessageFormattingService<AuthorEntryRow> messageFormattingService;

    private readonly IAuthorRepository repository;
    private readonly IUserDataProvider userDataProvider;

    /// <summary>
    /// Creates a new instance of <see cref="AuthorController"/>.
    /// </summary>
    /// <param name="limitProvider">The limit provider to retrieve the message character limits from.</param>
    /// <param name="userDataProvider">The provider to retrieve user data with.</param>
    /// <param name="repository">The repository to handle with the persistence of authors.</param>
    /// <param name="logger">The logger to log with.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    public AuthorController(IMessageCharacterLimitProvider limitProvider,
                            IUserDataProvider userDataProvider,
                            IAuthorRepository repository,
                            ILoggingService logger) : base(logger)
    {
        limitProvider.IsNotNull(nameof(limitProvider));
        userDataProvider.IsNotNull(nameof(userDataProvider));
        repository.IsNotNull(nameof(repository));

        messageFormattingService = new DataEntryCollectionMessageFormattingService<AuthorEntryRow>(
            limitProvider, header, entry => $"{entry.EntityId,-3} {entry.AuthorName,-50}");

        this.userDataProvider = userDataProvider;
        this.repository = repository;
    }

    public async Task<ControllerResult<string>> DeleteAuthorAsync(IUser user)
    {
        user.IsNotNull(nameof(user));

        try
        {
            ulong authorId = user.Id;
            UserData userData = await userDataProvider.GetUserDataAsync(authorId);
            await repository.DeleteAuthorAsync(authorId);

            return ControllerResult<string>.CreateControllerResultWithValidResult(
                string.Format(Resources.AuthorController_DeleteAuthor_All_data_of_Author_0_was_successfully_deleted, userData.Username));
        }
        catch (RepositoryDataDeleteException e)
        {
            return await HandleException<string>(e);
        }
    }

    public async Task<ControllerResult<IReadOnlyList<string>>> GetAllAuthorsAsync()
    {
        IReadOnlyCollection<AuthorRepositoryEntityData> entries = await repository.LoadAuthorsAsync();
        IEnumerable<AuthorEntryRow> rows = await CreateRows(entries);

        return ControllerResult<IReadOnlyList<string>>.CreateControllerResultWithValidResult(
            messageFormattingService.CreateMessages(rows, Resources.AuthorController_GetAuthors_No_saved_authors_are_found));
    }

    private async Task<IEnumerable<AuthorEntryRow>> CreateRows(IEnumerable<AuthorRepositoryEntityData> entries)
    {
        IEnumerable<Task<AuthorEntryRow>> authorEntryTasks = entries.Select(CreateAuthorEntryRow);
        return await Task.WhenAll(authorEntryTasks);
    }

    private async Task<AuthorEntryRow> CreateAuthorEntryRow(AuthorRepositoryEntityData entry)
    {
        if (entry.HasAuthorId)
        {
            UserData userData = await userDataProvider.GetUserDataAsync(entry.AuthorId!.Value);
            return new AuthorEntryRow
            {
                EntityId = entry.EntityId,
                AuthorName = userData.Username
            };
        }

        return new AuthorEntryRow
        {
            EntityId = entry.EntityId,
        };
    }

    private sealed record AuthorEntryRow
    {
        /// <summary>
        /// Gets the entity id of the author.
        /// </summary>
        public long EntityId { get; init; }

        /// <summary>
        /// Gets the name of the author of the recipe.
        /// </summary>
        public string AuthorName { get; init; } = "Unparseable author";
    }
}