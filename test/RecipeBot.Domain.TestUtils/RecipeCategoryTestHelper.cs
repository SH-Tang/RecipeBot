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

using System.Collections.Generic;
using RecipeBot.Domain.Data;

namespace RecipeBot.Domain.TestUtils;

/// <summary>
/// Helper class which can be used for asserting data related to <see cref="RecipeCategory"/>.
/// </summary>
public static class RecipeCategoryTestHelper
{
    /// <summary>
    /// Gets the string representations of each <see cref="RecipeCategory"/>.
    /// </summary>
    public static IReadOnlyDictionary<RecipeCategory, string> CategoryStringMapping =>
        new Dictionary<RecipeCategory, string>
        {
            {
                RecipeCategory.Meat, "Meat"
            },
            {
                RecipeCategory.Fish, "Fish"
            },
            {
                RecipeCategory.Vegetarian, "Vegetarian"
            },
            {
                RecipeCategory.Vegan, "Vegan"
            },
            {
                RecipeCategory.Drinks, "Drinks"
            },
            {
                RecipeCategory.Pastry, "Pastry"
            },
            {
                RecipeCategory.Dessert, "Dessert"
            },
            {
                RecipeCategory.Snack, "Snack"
            },
            {
                RecipeCategory.Other, "Other"
            }
        };
}