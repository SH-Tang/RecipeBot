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

using RecipeBot.Discord.Data;
using System.Collections.Generic;

namespace RecipeBot.Discord.TestUtils;

/// <summary>
/// Helper class which can be used for asserting data related to <see cref="DiscordRecipeCategory"/>
/// </summary>
public static class DiscordRecipeCategoryHelper
{
    /// <summary>
    /// Gets the string representations of each <see cref="DiscordRecipeCategory"/>.
    /// </summary>
    public static IReadOnlyDictionary<DiscordRecipeCategory, string> CategoryMapping =>
        new Dictionary<DiscordRecipeCategory, string>
        {
            {
                DiscordRecipeCategory.Meat, "Meat"
            },
            {
                DiscordRecipeCategory.Fish, "Fish"
            },
            {
                DiscordRecipeCategory.Vegetarian, "Vegetarian"
            },
            {
                DiscordRecipeCategory.Vegan, "Vegan"
            },
            {
                DiscordRecipeCategory.Drinks, "Drinks"
            },
            {
                DiscordRecipeCategory.Pastry, "Pastry"
            },
            {
                DiscordRecipeCategory.Dessert, "Dessert"
            },
            {
                DiscordRecipeCategory.Snack, "Snack"
            },
            {
                DiscordRecipeCategory.Other, "Other"
            }
        };
}