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

namespace RecipeBot.Persistence.Readers;

/// <summary>
/// Reader to get values of <see cref="RecipeCategory"/> from the persistence medium.
/// </summary>
internal static class RecipeCategoryReader
{
    /// <summary>
    /// Reads a <see cref="RecipeCategory"/> based on its input argument.
    /// </summary>
    /// <param name="category">The <see cref="PersistentRecipeCategory"/> to read from.</param>
    /// <returns>A <see cref="RecipeCategory"/>.</returns>
    /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/> is an invalid <see cref="PersistentRecipeCategory"/>.</exception>
    /// <exception cref="NotSupportedException">Thrown when <paramref name="category"/> is a valid <see cref="PersistentRecipeCategory"/>,
    /// but unsupported.</exception>
    public static RecipeCategory Read(PersistentRecipeCategory category)
    {
        category.IsValidEnum(nameof(category));

        switch (category)
        {
            case PersistentRecipeCategory.Meat:
                return RecipeCategory.Meat;
            case PersistentRecipeCategory.Fish:
                return RecipeCategory.Fish;
            case PersistentRecipeCategory.Vegetarian:
                return RecipeCategory.Vegetarian;
            case PersistentRecipeCategory.Vegan:
                return RecipeCategory.Vegan;
            case PersistentRecipeCategory.Drinks:
                return RecipeCategory.Drinks;
            case PersistentRecipeCategory.Pastry:
                return RecipeCategory.Pastry;
            case PersistentRecipeCategory.Dessert:
                return RecipeCategory.Dessert;
            case PersistentRecipeCategory.Snack:
                return RecipeCategory.Snack;
            case PersistentRecipeCategory.Other:
                return RecipeCategory.Other;
            default:
                throw new NotSupportedException();
        }
    }
}