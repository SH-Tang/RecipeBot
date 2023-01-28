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
using Discord.Common;
using RecipeBot.Discord.Controllers;
using RecipeBot.Discord.Data;
using RecipeBot.Domain.Repositories;
using RecipeBot.Domain.Repositories.Data;

namespace RecipeBot.Controllers;

/// <summary>
/// A concrete implementation of the <see cref="IRecipeEntriesController"/>.
/// </summary>
public class RecipeEntriesControllerMock : IRecipeEntriesController
{
    private readonly IMessageCharacterLimitProvider limitProvider;
    private readonly IRecipeDataEntryCollectionRepository repository;
    private readonly ILoggingService logger;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeEntriesControllerMock"/>.
    /// </summary>
    /// <param name="limitProvider">The limit provider to retrieve the message character limits from.</param>
    /// <param name="repository">The repository to handle with the persistence of recipe entries.</param>
    /// <param name="logger">The logger to log with.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    public RecipeEntriesControllerMock(IMessageCharacterLimitProvider limitProvider, 
        IRecipeDataEntryCollectionRepository repository, ILoggingService logger)
    {
        limitProvider.IsNotNull(nameof(limitProvider));
        repository.IsNotNull(nameof(repository));
        logger.IsNotNull(nameof(logger));

        this.limitProvider = limitProvider;
        this.repository = repository;
        this.logger = logger;
    }

    public async Task<ControllerResult<IReadOnlyList<string>>> ListAllRecipesAsync()
    {
        IReadOnlyList<RecipeEntryData> entries = await repository.LoadRecipeEntriesAsync();
        if (!entries.Any())
        {
            return new ControllerResult<IReadOnlyList<string>>(new[]
            {
                "No saved recipes are found."
            });
        }

        var header = $"{"Id",-3} {"Title",-50} {"Author",-50} ";
        string formattedEntries = string.Join($"{Environment.NewLine}", entries.Select(r => $"{r.Id,-3} {r.Title,-50} {r.AuthorName,-50}"));

        return new ControllerResult<IReadOnlyList<string>>(new []
        {
            Format.Code($"{header}{Environment.NewLine}{formattedEntries}")
        });
    }

    public Task<ControllerResult<IReadOnlyList<string>>> ListAllRecipesAsync(DiscordRecipeCategory category)
    {
        category.IsValidEnum(nameof(category));

        return Task.FromResult(new ControllerResult<IReadOnlyList<string>>("Not implemented yet"));
    }
}