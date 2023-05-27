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
using Discord.Common.Services;
using RecipeBot.Discord.Controllers;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Repositories;
using RecipeBot.Domain.Repositories.Data;
using RecipeBot.Properties;
using RecipeBot.Services;

namespace RecipeBot.Controllers;

/// <summary>
/// A concrete implementation of the <see cref="IRecipeTagEntriesController"/>.
/// </summary>
public class RecipeTagEntriesController : IRecipeTagEntriesController
{
    private static readonly string header = $"{"Id",-3} {"Tag",-50} ";

    private readonly DataEntryCollectionMessageFormattingService<RecipeTagEntryData> messageFormattingService;
    private readonly IRecipeTagEntryDataRepository repository;
    private readonly ILoggingService logger;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeTagEntriesController"/>.
    /// </summary>
    /// <param name="limitProvider">The limit provider to retrieve the message character limits from.</param>
    /// <param name="repository">The repository to handle with the persistence of recipe entries.</param>
    /// <param name="logger">The logger to log with.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    public RecipeTagEntriesController(IMessageCharacterLimitProvider limitProvider,
                                      IRecipeTagEntryDataRepository repository, ILoggingService logger)
    {
        limitProvider.IsNotNull(nameof(limitProvider));
        repository.IsNotNull(nameof(repository));
        logger.IsNotNull(nameof(logger));

        messageFormattingService = new DataEntryCollectionMessageFormattingService<RecipeTagEntryData>(
            limitProvider, header, entry => $"{entry.Id,-3} {entry.Tag,-50}");
        this.repository = repository;
        this.logger = logger;
    }

    public async Task<ControllerResult<IReadOnlyList<string>>> ListAllTagsAsync()
    {
        IReadOnlyList<RecipeTagEntryData> tags = await repository.LoadRecipeTagEntriesAsync();

        return ControllerResult<IReadOnlyList<string>>.CreateControllerResultWithValidResult(
            messageFormattingService.CreateMessages(tags, Resources.RecipeTagEntriesController_No_saved_tags_are_found));
    }

    public async Task<ControllerResult<string>> DeleteTagAsync(long idToDelete)
    {
        try
        {
            RecipeTagEntryData deletedTag = await repository.DeleteTagAsync(idToDelete);

            return ControllerResult<string>.CreateControllerResultWithValidResult(string.Format(Resources.RecipeTagEntriesController_DeleteTagAsync_Tag_0_with_Id_1_was_successfully_deleted,
                                                                                                deletedTag.Tag, deletedTag.Id));
        }
        catch (RepositoryDataDeleteException e)
        {
            logger.LogError(e);
            return ControllerResult<string>.CreateControllerResultWithError(e.Message);
        }
    }
}