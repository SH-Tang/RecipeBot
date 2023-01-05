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
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Common.Utils;
using Discord;
using Discord.Common;
using RecipeBot.Discord.Controllers;
using RecipeBot.Discord.Data;
using RecipeBot.Discord.Views;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Factories;
using RecipeBot.Domain.Models;
using RecipeBot.Domain.Repositories;
using RecipeBot.Exceptions;
using RecipeBot.Services;

namespace RecipeBot.Controllers;

/// <summary>
/// A concrete implementation of the <see cref="IRecipeController"/>.
/// </summary>
public class RecipeController : IRecipeController
{
    private readonly IRecipeRepository repository;
    private readonly ILoggingService logger;
    private readonly RecipeModelCreationService modelCreationService;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeController"/>.
    /// </summary>
    /// <param name="limitProvider">The limit provider to retrieve the character limits from.</param>
    /// <param name="repository">The repository to handle with the data persistence of recipes.</param>
    /// <param name="logger">The logger to log with.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    public RecipeController(IRecipeModelCharacterLimitProvider limitProvider,
                            IRecipeRepository repository, ILoggingService logger)
    {
        limitProvider.IsNotNull(nameof(limitProvider));
        repository.IsNotNull(nameof(repository));
        logger.IsNotNull(nameof(logger));

        modelCreationService = new RecipeModelCreationService(limitProvider);

        this.repository = repository;
        this.logger = logger;
    }

    public async Task<ControllerResult<Embed>> SaveRecipeAsync(RecipeModal modal, IUser user,
                                                               DiscordRecipeCategory category, IAttachment? attachment)
    {
        modal.IsNotNull(nameof(modal));
        user.IsNotNull(nameof(user));
        category.IsValidEnum(nameof(category));

        try
        {
            RecipeModel recipeModel = attachment == null
                                          ? modelCreationService.CreateRecipeModel(modal, user, category)
                                          : modelCreationService.CreateRecipeModel(modal, user, category, attachment);

            Task<Embed> embedTask = Task.Run(() => RecipeEmbedFactory.Create(recipeModel));
            Task[] tasks = 
            {
                embedTask,
                repository.SaveRecipeAsync(recipeModel)
            };

            await Task.WhenAll(tasks);

            return new ControllerResult<Embed>(embedTask.Result);
        }
        catch (ModelCreateException e)
        {
            return await HandleException(e);
        }
        catch (EmbedCreateException e)
        {
            return await HandleException(e);
        }
    }

    private async Task<ControllerResult<Embed>> HandleException(Exception e)
    {
        await logger.LogErrorAsync(e);
        return new ControllerResult<Embed>(e.Message);
    }
}