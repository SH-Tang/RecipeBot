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
using Common.Utils;
using Discord;
using RecipeBot.Discord.Exceptions;
using RecipeBot.Discord.Views;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Factories;
using RecipeBot.Domain.Models;

namespace RecipeBot.Discord.Services;

/// <summary>
/// Service to determine the response based on a <see cref="RecipeModal"/>.
/// </summary>
public class RecipeModalResponseService
{
    private readonly RecipeDomainEntityFactory recipeModelFactory;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeModalResponseService"/>.
    /// </summary>
    /// <param name="limitProvider">The limit provider to retrieve the character limits from.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="limitProvider"/> is <c>null</c>.</exception>
    public RecipeModalResponseService(IRecipeModelCharacterLimitProvider limitProvider)
    {
        limitProvider.IsNotNull(nameof(limitProvider));
        recipeModelFactory = new RecipeDomainEntityFactory(limitProvider);
    }

    /// <summary>
    /// Gets the modal response based on its input arguments.
    /// </summary>
    /// <param name="modal">The <see cref="RecipeModal"/> to get the response for.</param>
    /// <param name="user">The <see cref="IUser"/> to get the response for.</param>
    /// <returns>A response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    /// <exception cref="ModalResponseException">Thrown when the response could not be successfully determined.</exception>
    public Embed GetRecipeModalResponse(RecipeModal modal, IUser user)
    {
        modal.IsNotNull(nameof(modal));
        user.IsNotNull(nameof(user));

        var authorData = new AuthorData(user.Username, user.GetAvatarUrl());
        RecipeData recipeData = new RecipeDataBuilder(authorData, modal.RecipeTitle!, modal.Ingredients!, modal.CookingSteps!)
                                .AddNotes(modal.Notes)
                                .Build();

        return RecipeEmbedFactory.Create(GetRecipeModel(recipeData));
    }

    /// <summary>
    /// Gets the modal response based on its input arguments.
    /// </summary>
    /// <param name="modal">The <see cref="RecipeModal"/> to get the response for.</param>
    /// <param name="user">The <see cref="IUser"/> to get the response for.</param>
    /// <param name="attachment">The <see cref="IAttachment"/> to get the response with.</param>
    /// <returns>A response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="attachment"/> is invalid.</exception>
    /// <exception cref="ModalResponseException">Thrown when the response could not be successfully determined.</exception>
    public Embed GetRecipeModalResponse(RecipeModal modal, IUser user, IAttachment attachment)
    {
        modal.IsNotNull(nameof(modal));
        user.IsNotNull(nameof(user));

        var authorData = new AuthorData(user.Username, user.GetAvatarUrl());
        RecipeData recipeData = new RecipeDataBuilder(authorData, modal.RecipeTitle!, modal.Ingredients!, modal.CookingSteps!)
                                .AddNotes(modal.Notes)
                                .AddImage(attachment)
                                .Build();

        return RecipeEmbedFactory.Create(GetRecipeModel(recipeData));
    }

    /// <summary>
    /// Gets the <see cref="RecipeModel"/> based on the input arguments.
    /// </summary>
    /// <param name="recipeData">The <see cref="RecipeData"/> to get the <see cref="RecipeModel"/> with.</param>
    /// <returns>A <see cref="RecipeModel"/>.</returns>
    /// <exception cref="ModalResponseException">Thrown when the entity could not be successfully retrieved.</exception>
    private RecipeModel GetRecipeModel(RecipeData recipeData)
    {
        try
        {
            return recipeModelFactory.Create(recipeData);
        }
        catch (ModelCreateException e)
        {
            throw new ModalResponseException(e.Message, e);
        }
    }
}