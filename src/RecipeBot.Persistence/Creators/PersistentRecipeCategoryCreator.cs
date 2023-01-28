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
using RecipeBot.Domain.Data;
using RecipeBot.Persistence.Entities;

namespace RecipeBot.Persistence.Creators;

/// <summary>
/// Creator to create <see cref="PersistentRecipeCategory"/>.
/// </summary>
internal static class PersistentRecipeCategoryCreator
{
    /// <summary>
    /// Creates a <see cref="PersistentRecipeCategory"/> based on its input argument.
    /// </summary>
    /// <param name="category">The <see cref="RecipeCategory"/> to create the <see cref="PersistentRecipeCategory"/> with.</param>
    /// <returns>A <see cref="PersistentRecipeCategory"/>.</returns>
    /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/> is an invalid <see cref="RecipeCategory"/>.</exception>
    /// <exception cref="NotSupportedException">Thrown when <paramref name="category"/> is a valid <see cref="RecipeCategory"/>,
    /// but unsupported.</exception>
    public static PersistentRecipeCategory Create(RecipeCategory category)
    {
        category.IsValidEnum(nameof(category));

        switch (category)
        {
            case RecipeCategory.Meat:
                return PersistentRecipeCategory.Meat;
            case RecipeCategory.Fish:
                return PersistentRecipeCategory.Fish;
            case RecipeCategory.Vegetarian:
                return PersistentRecipeCategory.Vegetarian;
            case RecipeCategory.Vegan:
                return PersistentRecipeCategory.Vegan;
            case RecipeCategory.Drinks:
                return PersistentRecipeCategory.Drinks;
            case RecipeCategory.Pastry:
                return PersistentRecipeCategory.Pastry;
            case RecipeCategory.Dessert:
                return PersistentRecipeCategory.Dessert;
            case RecipeCategory.Snack:
                return PersistentRecipeCategory.Snack;
            case RecipeCategory.Other:
                return PersistentRecipeCategory.Other;
            default:
                throw new NotSupportedException();
        }
    }
}