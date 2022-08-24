// Copyright (C) 2022 Dennis Tang. All rights reserved.
//
// This file is part of WeekendBot.
//
// WeekendBot is free software: you can redistribute it and/or modify
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
using Discord;
using WeekendBot.Domain.Data;
using WeekendBot.Domain.Entities;
using WeekendBot.Domain.Exceptions;
using WeekendBot.Domain.Factories;
using WeekendBot.Utils;

namespace WeekendBot.Discord;

/// <summary>
/// Service to determine the response  based on a <see cref="RecipeModal"/>.
/// </summary>
public class RecipeModalResponseService
{
    private readonly RecipeDomainEntityFactory recipeDomainEntityFactory;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeModalResponseService"/>.
    /// </summary>
    /// <param name="limitProvider"></param>
    public RecipeModalResponseService(IRecipeDomainEntityCharacterLimitProvider limitProvider)
    {
        limitProvider.IsNotNull(nameof(limitProvider));
        recipeDomainEntityFactory = new RecipeDomainEntityFactory(limitProvider);
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

        return RecipeEmbedFactory.Create(GetRecipeDomainEntity(recipeData));
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

        return RecipeEmbedFactory.Create(GetRecipeDomainEntity(recipeData));
    }

    /// <summary>
    /// Gets the <see cref="RecipeDomainEntity"/> based on the input arguments.
    /// </summary>
    /// <param name="recipeData">The <see cref="RecipeData"/> to get the <see cref="RecipeDomainEntity"/> with.</param>
    /// <returns>A <see cref="RecipeDomainEntity"/>.</returns>
    /// <exception cref="ModalResponseException">Thrown when the entity could not be successfully retrieved.</exception>
    private RecipeDomainEntity GetRecipeDomainEntity(RecipeData recipeData)
    {
        try
        {
            return recipeDomainEntityFactory.Create(recipeData);
        }
        catch (DomainEntityCreateException e)
        {
            throw new ModalResponseException(e.Message, e);
        }
    }
}