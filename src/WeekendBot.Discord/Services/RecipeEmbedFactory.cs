﻿// Copyright (C) 2022 Dennis Tang. All rights reserved.
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
using System.Collections.Generic;
using Discord;
using WeekendBot.Discord.Exceptions;
using WeekendBot.Discord.Properties;
using WeekendBot.Domain.Entities;
using WeekendBot.Utils;

namespace WeekendBot.Discord.Services;

/// <summary>
/// Factory to create instances of <see cref="Embed"/> for recipes.
/// </summary>
public static class RecipeEmbedFactory
{
    /// <summary>
    /// Creates an <see cref="Embed"/> based on its input arguments.
    /// </summary>
    /// <param name="recipe">The <see cref="RecipeDomainEntity"/> to create the <see cref="Embed"/> with.</param>
    /// <returns>An <see cref="Embed"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="recipe"/> is <c>null</c>.</exception>
    /// <exception cref="ModalResponseException">Thrown when the response could not be successfully determined.</exception>
    public static Embed Create(RecipeDomainEntity recipe)
    {
        recipe.IsNotNull(nameof(recipe));

        try
        {
            EmbedBuilder embedBuilder = CreateConfiguredEmbedBuilder(recipe);
            return embedBuilder.Build();
        }
        catch (InvalidOperationException e)
        {
            throw new ModalResponseException(string.Format(Resources.RecipeModal_response_could_not_be_determined_reason_0_, e.Message), e);
        }
    }

    private static EmbedBuilder CreateConfiguredEmbedBuilder(RecipeDomainEntity recipeData)
    {
        AuthorDomainEntity authorData = recipeData.AuthorEntity;
        EmbedBuilder embedBuilder = new EmbedBuilder().WithAuthor(authorData.AuthorName, authorData.AuthorImageUrl)
                                                      .WithTitle(recipeData.Title);
        if (!string.IsNullOrWhiteSpace(recipeData.RecipeImageUrl))
        {
            embedBuilder.WithImageUrl(recipeData.RecipeImageUrl);
        }

        ConfigureFields(embedBuilder, recipeData.RecipeFieldEntities);

        return embedBuilder;
    }

    private static void ConfigureFields(EmbedBuilder embedBuilder, IEnumerable<RecipeFieldDomainEntity> fieldDomainEntities)
    {
        try
        {
            foreach (RecipeFieldDomainEntity fieldDomainEntity in fieldDomainEntities)
            {
                embedBuilder.AddField(fieldDomainEntity.FieldName, fieldDomainEntity.FieldData);
            }
        }
        catch (ArgumentException e)
        {
            throw new ModalResponseException(string.Format(Resources.RecipeModal_response_could_not_be_determined_reason_0_, e.Message), e);
        }
    }
}