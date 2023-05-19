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
using System.Threading.Tasks;
using Common.Utils;
using Discord;
using Discord.Common;
using Discord.Common.Providers;
using Discord.Common.Utils;
using RecipeBot.Discord.Controllers;
using RecipeBot.Discord.Data;
using RecipeBot.Discord.Views;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Factories;
using RecipeBot.Domain.Models;
using RecipeBot.Domain.Repositories;
using RecipeBot.Domain.Repositories.Data;
using RecipeBot.Exceptions;
using RecipeBot.Properties;
using RecipeBot.Services;

namespace RecipeBot.Controllers;

/// <summary>
/// A concrete implementation of the <see cref="IRecipeController"/>.
/// </summary>
public class RecipeController : IRecipeController
{
    private readonly ILoggingService logger;
    private readonly RecipeModelCreationService modelCreationService;
    private readonly RecipeModelFactory modelFactory;
    private readonly IRecipeRepository repository;
    private readonly IUserDataProvider userDataProvider;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeController"/>.
    /// </summary>
    /// <param name="limitProvider">The limit provider to retrieve the character limits from.</param>
    /// <param name="userDataProvider">The provider to retrieve user data with.</param>
    /// <param name="repository">The repository to handle with the persistence of recipes.</param>
    /// <param name="logger">The logger to log with.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    public RecipeController(IRecipeModelCharacterLimitProvider limitProvider,
                            IUserDataProvider userDataProvider,
                            IRecipeRepository repository,
                            ILoggingService logger)
    {
        limitProvider.IsNotNull(nameof(limitProvider));
        userDataProvider.IsNotNull(nameof(userDataProvider));
        repository.IsNotNull(nameof(repository));
        logger.IsNotNull(nameof(logger));

        modelCreationService = new RecipeModelCreationService(limitProvider);
        modelFactory = new RecipeModelFactory(limitProvider);

        this.userDataProvider = userDataProvider;
        this.repository = repository;
        this.logger = logger;
    }

    public async Task<ControllerResult<Embed>> SaveRecipeAsync(RecipeModal modal, IUser user,
                                                               DiscordRecipeCategory category)
    {
        modal.IsNotNull(nameof(modal));
        user.IsNotNull(nameof(user));
        category.IsValidEnum(nameof(category));

        try
        {
            RecipeModel recipeModel = modelCreationService.CreateRecipeModel(modal, user, category);

            UserData author = IUserHelper.Create(user);
            Task<Embed> embedTask = Task.Run(() => RecipeEmbedFactory.Create(recipeModel, author));
            Task[] tasks =
            {
                embedTask,
                repository.SaveRecipeAsync(recipeModel)
            };

            await Task.WhenAll(tasks);

            return ControllerResult<Embed>.CreateControllerResultWithValidResult(embedTask.Result);
        }
        catch (ModelCreateException e)
        {
            return await HandleException<Embed>(e);
        }
        catch (EmbedCreateException e)
        {
            return await HandleException<Embed>(e);
        }
        catch (RepositoryDataSaveException e)
        {
            return await HandleException<Embed>(e);
        }
    }

    public async Task<ControllerResult<string>> DeleteRecipeAsync(long idToDelete)
    {
        try
        {
            RecipeEntryData deletedRecipe = await repository.DeleteRecipeAsync(idToDelete);

            UserData userData = await userDataProvider.GetUserDataAsync(deletedRecipe.AuthorId);
            return ControllerResult<string>.CreateControllerResultWithValidResult(string.Format(Resources.RecipeController_DeleteRecipeAsync_RecipeTitle_0_with_RecipeId_1_and_AuthorName_2_was_succesfully_deleted,
                                                                                                deletedRecipe.Title, deletedRecipe.Id, userData.Username));
        }
        catch (RepositoryDataDeleteException e)
        {
            return await HandleException<string>(e);
        }
    }

    public async Task<ControllerResult<string>> DeleteRecipeAsync(long idToDelete, IUser user)
    {
        user.IsNotNull(nameof(user));

        try
        {
            ulong authorId = user.Id;
            RecipeEntryData deletedRecipe = await repository.DeleteRecipeAsync(idToDelete, authorId);

            UserData userData = await userDataProvider.GetUserDataAsync(authorId);
            return ControllerResult<string>.CreateControllerResultWithValidResult(string.Format(Resources.RecipeController_DeleteRecipeAsync_RecipeTitle_0_with_RecipeId_1_and_AuthorName_2_was_succesfully_deleted,
                                                                                                deletedRecipe.Title, deletedRecipe.Id, userData.Username));
        }
        catch (RepositoryDataDeleteException e)
        {
            return await HandleException<string>(e);
        }
    }

    public async Task<ControllerResult<Embed>> GetRecipeAsync(long idToRetrieve)
    {
        try
        {
            RecipeData recipeData = await repository.GetRecipeAsync(idToRetrieve);
            RecipeModel modelData = modelFactory.Create(recipeData);
            UserData userData = await userDataProvider.GetUserDataAsync(recipeData.AuthorId);

            return ControllerResult<Embed>.CreateControllerResultWithValidResult(RecipeEmbedFactory.Create(modelData, userData));
        }
        catch (ModelCreateException e)
        {
            return await HandleException<Embed>(e);
        }
        catch (EmbedCreateException e)
        {
            return await HandleException<Embed>(e);
        }
        catch (RepositoryDataLoadException e)
        {
            return await HandleException<Embed>(e);
        }
    }

    private async Task<ControllerResult<TResult>> HandleException<TResult>(Exception e) where TResult : class
    {
        await logger.LogErrorAsync(e);
        return ControllerResult<TResult>.CreateControllerResultWithError(e.Message);
    }
}