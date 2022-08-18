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
using WeekendBot.Domain;
using WeekendBot.Domain.Data;
using WeekendBot.Utils;

namespace WeekendBot.Services;

/// <summary>
/// Service to determine the response  based on a <see cref="RecipeModal"/>.
/// </summary>
public static class RecipeModalResponseService
{
    /// <summary>
    /// Gets the modal response based on its input arguments.
    /// </summary>
    /// <param name="modal">The <see cref="RecipeModal"/> to get the response for.</param>
    /// <param name="user">The <see cref="IUser"/> to get the response for.</param>
    /// <returns>A response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    /// <exception cref="ModalResponseException">Thrown when the response could not be successfully determined.</exception>
    public static Embed GetRecipeModalResponse(RecipeModal modal, IUser user)
    {
        modal.IsNotNull(nameof(modal));
        user.IsNotNull(nameof(user));

        var authorData = new AuthorData(user.Username, user.GetAvatarUrl());
        RecipeDataBuilder recipeDataBuilder = new RecipeDataBuilder(authorData, modal.RecipeTitle!, modal.Ingredients!, modal.CookingSteps!)
            .AddNotes(modal.Notes);

        return RecipeEmbedFactory.Create(recipeDataBuilder.Build());
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
    public static Embed GetRecipeModalResponse(RecipeModal modal, IUser user, IAttachment attachment)
    {
        modal.IsNotNull(nameof(modal));
        user.IsNotNull(nameof(user));

        var authorData = new AuthorData(user.Username, user.GetAvatarUrl());
        RecipeDataBuilder recipeDataBuilder = new RecipeDataBuilder(authorData, modal.RecipeTitle!, modal.Ingredients!, modal.CookingSteps!)
                                              .AddNotes(modal.Notes)
                                              .AddImage(attachment);

        return RecipeEmbedFactory.Create(recipeDataBuilder.Build());
    }
}