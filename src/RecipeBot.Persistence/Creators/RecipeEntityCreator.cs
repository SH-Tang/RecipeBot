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
using RecipeBot.Domain.Models;
using RecipeBot.Persistence.Entities;

namespace RecipeBot.Persistence.Creators;

/// <summary>
/// Creator to create instances of <see cref="RecipeEntity"/>.
/// </summary>
internal static class RecipeEntityCreator
{
    /// <summary>
    /// Creates an <see cref="RecipeEntity"/> based on its input arguments.
    /// </summary>
    /// <param name="model">The <see cref="RecipeModel"/> to create the entity with.</param>
    /// <param name="authorEntity">The associated <see cref="AuthorEntity"/> that belongs to the entity.</param>
    /// <param name="recipeTagEntities">The associated collection of <see cref="RecipeTagEntity"/>
    /// that belongs to the entity.</param>
    /// <returns>A <see cref="RecipeEntity"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    public static RecipeEntity Create(RecipeModel model,
                                      AuthorEntity authorEntity,
                                      ICollection<RecipeTagEntity> recipeTagEntities)
    {
        model.IsNotNull(nameof(model));
        authorEntity.IsNotNull(nameof(authorEntity));
        recipeTagEntities.IsNotNull(nameof(recipeTagEntities));

        return new RecipeEntity
        {
            RecipeTitle = model.Title,
            RecipeCategory = PersistentRecipeCategoryCreator.Create(model.RecipeCategory),
            Author = authorEntity,
            RecipeFields = Create(model.RecipeFields),
            Tags = recipeTagEntities
        };
    }

    private static ICollection<RecipeFieldEntity> Create(IEnumerable<RecipeFieldModel> recipeFieldModels)
    {
        byte i = 0;
        return recipeFieldModels.Select(recipeField => new RecipeFieldEntity
        {
            RecipeFieldName = recipeField.FieldName,
            RecipeFieldData = recipeField.FieldData,
            Order = i++
        }).ToArray();
    }
}