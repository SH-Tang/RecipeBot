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
using System.ComponentModel;
using Common.Utils;
using Discord;
using RecipeBot.Discord.Data;
using RecipeBot.Discord.Views;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Factories;
using RecipeBot.Domain.Models;

namespace RecipeBot.Services;

/// <summary>
/// Service to create instances of <see cref="RecipeModel"/>.
/// </summary>
internal class RecipeModelCreationService
{
    private readonly RecipeModelFactory recipeModelFactory;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeModelCreationService"/>.
    /// </summary>
    /// <param name="limitProvider">The limit provider to retrieve the character limits from.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="limitProvider"/> is <c>null</c>.</exception>
    public RecipeModelCreationService(IRecipeModelCharacterLimitProvider limitProvider)
    {
        limitProvider.IsNotNull(nameof(limitProvider));
        recipeModelFactory = new RecipeModelFactory(limitProvider);
    }

    /// <summary>
    /// Gets the <see cref="RecipeModel"/> based on its input arguments.
    /// </summary>
    /// <param name="modal">The <see cref="RecipeModal"/> to create the model with.</param>
    /// <param name="user">The <see cref="IUser"/> to create the model with.</param>
    /// <param name="recipeCategory">The <see cref="DiscordRecipeCategory"/> the recipe belongs to.</param>
    /// <returns>A <see cref="RecipeModel"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="recipeCategory"/>
    /// is an invalid <see cref="DiscordRecipeCategory"/>.</exception>
    /// <exception cref="ModelCreateException">Thrown when the model could not be successfully created.</exception>
    public RecipeModel CreateRecipeModel(RecipeModal modal, IUser user, DiscordRecipeCategory recipeCategory)
    {
        modal.IsNotNull(nameof(modal));
        user.IsNotNull(nameof(user));

        var authorData = new AuthorData(user.Username, user.GetAvatarUrl());
        RecipeData recipeData = new RecipeDataBuilder(authorData, recipeCategory, modal.RecipeTitle!, modal.Ingredients!, modal.CookingSteps!)
                                .AddNotes(modal.Notes)
                                .AddTags(modal.Tags)
                                .Build();

        return recipeModelFactory.Create(recipeData);
    }

    /// <summary>
    /// Gets the <see cref="RecipeModel"/> based on its input arguments.
    /// </summary>
    /// <param name="modal">The <see cref="RecipeModal"/> to create the model with.</param>
    /// <param name="user">The <see cref="IUser"/> to create the model with.</param>
    /// <param name="recipeCategory">The <see cref="DiscordRecipeCategory"/> the recipe belongs to.</param>
    /// <param name="attachment">The <see cref="IAttachment"/> to create the model with.</param>
    /// <returns>A <see cref="RecipeModel"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="attachment"/> is invalid.</exception>
    /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="recipeCategory"/>
    /// is an invalid <see cref="DiscordRecipeCategory"/>.</exception>
    /// <exception cref="ModelCreateException">Thrown when the model could not be successfully created.</exception>
    public RecipeModel CreateRecipeModel(RecipeModal modal, IUser user, DiscordRecipeCategory recipeCategory, IAttachment? attachment)
    {
        modal.IsNotNull(nameof(modal));
        user.IsNotNull(nameof(user));

        var authorData = new AuthorData(user.Username, user.GetAvatarUrl());
        RecipeData recipeData = new RecipeDataBuilder(authorData, recipeCategory, modal.RecipeTitle!, modal.Ingredients!, modal.CookingSteps!)
                                .AddNotes(modal.Notes)
                                .AddTags(modal.Tags)
                                .AddImage(attachment)
                                .Build();

        return recipeModelFactory.Create(recipeData);
    }
}