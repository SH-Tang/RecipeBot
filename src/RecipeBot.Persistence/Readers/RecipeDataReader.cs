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
using Common.Utils;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Persistence.Entities;
using RecipeBot.Persistence.Properties;

namespace RecipeBot.Persistence.Readers;

/// <summary>
/// Reader to get instances of <see cref="RecipeData"/> from the persistence medium.
/// </summary>
internal static class RecipeDataReader
{
    /// <summary>
    /// Reads a <see cref="RecipeData"/> based on its input argument.
    /// </summary>
    /// <param name="entity">The <see cref="RecipeEntity"/> to read from.</param>
    /// <returns>A <see cref="RecipeData"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
    /// <exception cref="RepositoryDataLoadException">Thrown when the <see cref="RecipeData"/> could not be successfully loaded.</exception>
    public static RecipeData Read(RecipeEntity entity)
    {
        entity.IsNotNull(nameof(entity));

        string authorId = entity.Author.AuthorId;
        try
        {
            IEnumerable<RecipeFieldData> recipeFieldsData = entity.RecipeFields
                                                                  .OrderBy(f => f.Order)
                                                                  .Select(CreateRecipeFieldData)
                                                                  .ToArray();

            var recipeData = new RecipeData(ulong.Parse(authorId), recipeFieldsData, entity.RecipeTitle, RecipeCategoryReader.Read(entity.RecipeCategory));
            if (entity.Tags.Any())
            {
                string tags = string.Join(", ", entity.Tags.OrderBy(t => t.Order).Select(t => t.Tag.Tag));
                recipeData.Tags = tags;
            }

            return recipeData;
        }
        catch (Exception e) when (e is FormatException || e is OverflowException)
        {
            throw new RepositoryDataLoadException(string.Format(Resources.RecipeEntityId_0_unsuccessfully_loaded_due_to_invalid_AuthorId_1, entity.RecipeEntityId, authorId), e);
        }
    }

    private static RecipeFieldData CreateRecipeFieldData(RecipeFieldEntity f)
    {
        return new RecipeFieldData(f.RecipeFieldName, f.RecipeFieldData);
    }
}