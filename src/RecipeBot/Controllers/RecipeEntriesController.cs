﻿// Copyright (C) 2022 Dennis Tang. All rights reserved.
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common.Utils;
using Discord;
using Discord.Common.Providers;
using Discord.Common.Services;
using RecipeBot.Discord.Controllers;
using RecipeBot.Discord.Data;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Repositories;
using RecipeBot.Domain.Repositories.Data;
using RecipeBot.Properties;
using RecipeBot.Services;

namespace RecipeBot.Controllers;

/// <summary>
/// A concrete implementation of the <see cref="IRecipeEntriesController"/>.
/// </summary>
public class RecipeEntriesController : ControllerBase, IRecipeEntriesController
{
    private static readonly string header = $"{"Id",-3} {"Title",-50} {"Author",-50} ";

    private readonly DataEntryCollectionMessageFormattingService<RecipeEntryRow> messageFormattingService;
    private readonly IUserDataProvider userDataProvider;
    private readonly IRecipeCollectionRepository repository;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeEntriesController"/>.
    /// </summary>
    /// <param name="limitProvider">The limit provider to retrieve the message character limits from.</param>
    /// <param name="userDataProvider">The provider to retrieve user data with.</param>
    /// <param name="repository">The repository to handle with the persistence of recipe entries.</param>
    /// <param name="logger">The logger to log with.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    public RecipeEntriesController(IMessageCharacterLimitProvider limitProvider,
                                   IUserDataProvider userDataProvider,
                                   IRecipeCollectionRepository repository,
                                   ILoggingService logger) : base(logger)
    {
        limitProvider.IsNotNull(nameof(limitProvider));
        userDataProvider.IsNotNull(nameof(userDataProvider));
        repository.IsNotNull(nameof(repository));

        messageFormattingService = new DataEntryCollectionMessageFormattingService<RecipeEntryRow>(
            limitProvider, header, entry => $"{entry.EntityId,-3} {entry.Title,-50} {entry.AuthorName,-50}");
        this.userDataProvider = userDataProvider;
        this.repository = repository;
    }

    public async Task<ControllerResult<IReadOnlyList<string>>> GetAllRecipesAsync()
    {
        try
        {
            IReadOnlyList<RecipeRepositoryEntityData> entries = await repository.LoadRecipeEntriesAsync();
            IEnumerable<RecipeEntryRow> rows = await CreateRows(entries);

            return ControllerResult<IReadOnlyList<string>>.CreateControllerResultWithValidResult(
                messageFormattingService.CreateMessages(rows, Resources.RecipeEntriesController_GetRecipes_No_saved_recipes_are_found));
        }
        catch (RepositoryDataLoadException e)
        {
            return HandleException<IReadOnlyList<string>>(e);
        }
    }

    public async Task<ControllerResult<IReadOnlyList<string>>> GetAllRecipesByCategoryAsync(DiscordRecipeCategory category)
    {
        category.IsValidEnum(nameof(category));

        try
        {
            RecipeCategory repositoryCategory = RecipeCategoryConverter.ConvertFrom(category);
            IReadOnlyList<RecipeRepositoryEntityData> entries = await repository.LoadRecipeEntriesByCategoryAsync(repositoryCategory);
            IEnumerable<RecipeEntryRow> rows = await CreateRows(entries);

            return ControllerResult<IReadOnlyList<string>>.CreateControllerResultWithValidResult(
                messageFormattingService.CreateMessages(rows, Resources.RecipeEntriesController_GetAllRecipesAsync_No_saved_recipes_are_found_with_category));
        }
        catch (RepositoryDataLoadException e)
        {
            return HandleException<IReadOnlyList<string>>(e);
        }
    }

    public async Task<ControllerResult<IReadOnlyList<string>>> GetAllRecipesByTagAsync(string tag)
    {
        try
        {
            string postProcessTag = Regex.Replace(tag, @"\s+", "", RegexOptions.None, TimeSpan.FromMilliseconds(100)).ToLower();
            IReadOnlyList<RecipeRepositoryEntityData> entries = await repository.LoadRecipeEntriesByTagAsync(postProcessTag);
            IEnumerable<RecipeEntryRow> rows = await CreateRows(entries);

            return ControllerResult<IReadOnlyList<string>>.CreateControllerResultWithValidResult(
                messageFormattingService.CreateMessages(rows, string.Format(Resources.RecipeEntriesController_GetAllRecipesByTagAsync_No_saved_recipes_are_found_with_Tag_0_, tag)));
        }
        catch (RepositoryDataLoadException e)
        {
            return HandleException<IReadOnlyList<string>>(e);
        }
    }

    public async Task<ControllerResult<IReadOnlyList<string>>> GetAllRecipesByTagIdAsync(long tagId)
    {
        try
        {
            IReadOnlyList<RecipeRepositoryEntityData> entries = await repository.LoadRecipeEntriesByTagEntityIdAsync(tagId);
            IEnumerable<RecipeEntryRow> rows = await CreateRows(entries);
            return ControllerResult<IReadOnlyList<string>>.CreateControllerResultWithValidResult(
                messageFormattingService.CreateMessages(rows, string.Format(Resources.RecipeEntriesController_GetAllRecipesByTagAsync_No_saved_recipes_are_found_with_TagId_0_, tagId)));
        }
        catch (RepositoryDataLoadException e)
        {
            return HandleException<IReadOnlyList<string>>(e);
        }
    }

    public async Task<ControllerResult<IReadOnlyList<string>>> GetAllRecipesByUserAsync(IUser user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        try
        {
            IReadOnlyList<RecipeRepositoryEntityData> entries = await repository.LoadRecipeEntriesByAuthorIdAsync(user.Id);
            IEnumerable<RecipeEntryRow> rows = await CreateRows(entries);

            return ControllerResult<IReadOnlyList<string>>.CreateControllerResultWithValidResult(
                messageFormattingService.CreateMessages(rows, Resources.RecipeEntriesController_GetRecipes_No_saved_recipes_are_found));
        }
        catch (RepositoryDataLoadException e)
        {
            return HandleException<IReadOnlyList<string>>(e);
        }
    }

    private async Task<IEnumerable<RecipeEntryRow>> CreateRows(IEnumerable<RecipeRepositoryEntityData> entries)
    {
        IEnumerable<Task<Tuple<ulong, string>>> authorEntryTasks = entries.Select(CreateAuthorEntry);
        Tuple<ulong, string>[] authorEntries = await Task.WhenAll(authorEntryTasks);
        Dictionary<ulong, string> authorLookup =
            authorEntries.DistinctBy(a => a.Item1).ToDictionary(entry => entry.Item1, entry => entry.Item2);

        return entries.Select(e => new RecipeEntryRow
        {
            EntityId = e.EntityId,
            Title = e.Title,
            AuthorName = authorLookup[e.AuthorId]
        }).ToArray();
    }

    private async Task<Tuple<ulong, string>> CreateAuthorEntry(RecipeRepositoryEntityData entry)
    {
        ulong authorId = entry.AuthorId;
        UserData userData = await userDataProvider.GetUserDataAsync(authorId);

        return new Tuple<ulong, string>(authorId, userData.Username);
    }

    private sealed record RecipeEntryRow
    {
        /// <summary>
        /// Gets the entity id of the recipe.
        /// </summary>
        public long EntityId { get; init; }

        /// <summary>
        /// Gets the title of the recipe.
        /// </summary>
        public string Title { get; init; } = null!;

        /// <summary>
        /// Gets the name of the author of the recipe.
        /// </summary>
        public string AuthorName { get; init; } = null!;
    }
}