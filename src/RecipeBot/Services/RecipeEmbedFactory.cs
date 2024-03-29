﻿// Copyright (C) 2022 Dennis Tang. All rights reserved.
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
using Common.Utils;
using Discord;
using Discord.Common.Providers;
using RecipeBot.Domain.Models;
using RecipeBot.Exceptions;
using RecipeBot.Properties;

namespace RecipeBot.Services;

/// <summary>
/// Factory to create instances of <see cref="Embed"/> for recipes.
/// </summary>
internal static class RecipeEmbedFactory
{
    /// <summary>
    /// Creates an <see cref="Embed"/> based on its input arguments.
    /// </summary>
    /// <param name="recipe">The <see cref="RecipeModel"/> to create the <see cref="Embed"/> with.</param>
    /// <param name="author">The author associated with <paramref name="recipe"/>.</param>
    /// <returns>An <see cref="Embed"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
    /// <exception cref="EmbedCreateException">Thrown when the <see cref="Embed"/> could not be successfully created.</exception>
    public static Embed Create(RecipeModel recipe, UserData author)
    {
        recipe.IsNotNull(nameof(recipe));
        author.IsNotNull(nameof(author));

        try
        {
            EmbedBuilder embedBuilder = CreateConfiguredEmbedBuilder(recipe, author);
            return embedBuilder.Build();
        }
        catch (InvalidOperationException e)
        {
            throw new EmbedCreateException(string.Format(Resources.Embed_could_not_be_created_reason_0_, e.Message), e);
        }
    }

    private static EmbedBuilder CreateConfiguredEmbedBuilder(RecipeModel recipeData, UserData author)
    {
        EmbedBuilder embedBuilder = new EmbedBuilder().WithTitle(recipeData.Title)
                                                      .WithAuthor(author.Username, author.UserImageUrl)
                                                      .WithColor(RecipeCategoryConverter.ConvertTo(recipeData.RecipeCategory));
        RecipeTagsModelWrapper tagData = recipeData.RecipeTags;
        if (tagData.Tags.Any())
        {
            embedBuilder.WithFooter(tagData.ToString());
        }

        ConfigureFields(embedBuilder, recipeData.RecipeFields);

        return embedBuilder;
    }

    private static void ConfigureFields(EmbedBuilder embedBuilder, IEnumerable<RecipeFieldModel> recipeFields)
    {
        try
        {
            foreach (RecipeFieldModel recipeField in recipeFields)
            {
                embedBuilder.AddField(recipeField.FieldName, recipeField.FieldData);
            }
        }
        catch (ArgumentException e)
        {
            throw new EmbedCreateException(string.Format(Resources.Embed_could_not_be_created_reason_0_, e.Message), e);
        }
    }
}