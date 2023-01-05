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
using Microsoft.Extensions.Logging;
using RecipeBot.Discord.Controllers;
using RecipeBot.Discord.Data;
using RecipeBot.Discord.Views;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Factories;
using RecipeBot.Domain.Models;
using RecipeBot.Exceptions;
using RecipeBot.Services;

namespace RecipeBot.Controllers;

/// <summary>
/// A concrete implementation of the <see cref="IRecipeController"/>.
/// </summary>
public class RecipeController : IRecipeController
{
    private readonly ILoggingService logger;
    private readonly RecipeModelCreationService modelCreationService;

    public RecipeController(IRecipeModelCharacterLimitProvider limitProvider,
                            ILoggingService logger)
    {
        limitProvider.IsNotNull(nameof(limitProvider));
        logger.IsNotNull(nameof(logger));

        modelCreationService = new RecipeModelCreationService(limitProvider);

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

            return new ControllerResult<Embed>(RecipeEmbedFactory.Create(recipeModel));
        }
        catch (ModelCreateException e)
        {
            await logger.LogErrorAsync(e);
            return new ControllerResult<Embed>(e.Message);
        }
        catch (ModalResponseException e)
        {
            await logger.LogErrorAsync(e);
            return new ControllerResult<Embed>(e.Message);
        }
    }
}