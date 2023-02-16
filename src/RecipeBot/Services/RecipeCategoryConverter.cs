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
using RecipeBot.Domain.Data;

namespace RecipeBot.Services;

/// <summary>
/// Converter for <see cref="RecipeCategory"/>
/// </summary>
internal static class RecipeCategoryConverter
{
    /// <summary>
    /// Converts its input argument to a <see cref="RecipeCategory"/>.
    /// </summary>
    /// <param name="recipeCategory">The <see cref="DiscordRecipeCategory"/> to convert from.</param>
    /// <returns>A <see cref="RecipeCategory"/>.</returns>
    /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="recipeCategory"/>
    /// is an invalid <see cref="DiscordRecipeCategory"/>.</exception>
    /// <exception cref="NotSupportedException">Thrown when <paramref name="recipeCategory"/> is a valid <see cref="DiscordRecipeCategory"/>,
    /// but unsupported.</exception>
    public static RecipeCategory ConvertFrom(DiscordRecipeCategory recipeCategory)
    {
        recipeCategory.IsValidEnum(nameof(recipeCategory));

        switch (recipeCategory)
        {
            case DiscordRecipeCategory.Meat:
                return RecipeCategory.Meat;
            case DiscordRecipeCategory.Fish:
                return RecipeCategory.Fish;
            case DiscordRecipeCategory.Vegetarian:
                return RecipeCategory.Vegetarian;
            case DiscordRecipeCategory.Vegan:
                return RecipeCategory.Vegan;
            case DiscordRecipeCategory.Drinks:
                return RecipeCategory.Drinks;
            case DiscordRecipeCategory.Pastry:
                return RecipeCategory.Pastry;
            case DiscordRecipeCategory.Dessert:
                return RecipeCategory.Dessert;
            case DiscordRecipeCategory.Snack:
                return RecipeCategory.Snack;
            case DiscordRecipeCategory.Other:
                return RecipeCategory.Other;
            default:
                throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Converts a <see cref="RecipeCategory"/> to a target type.
    /// </summary>
    /// <param name="recipeCategory">The <see cref="RecipeCategory"/> to convert to another type.</param>
    /// <returns>A <see cref="Color"/>.</returns>
    /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="recipeCategory"/>
    /// is an invalid <see cref="RecipeCategory"/>.</exception>
    /// <exception cref="NotSupportedException">Thrown when <paramref name="recipeCategory"/> is a valid <see cref="RecipeCategory"/>,
    /// but unsupported.</exception>
    public static Color ConvertTo(RecipeCategory recipeCategory)
    {
        recipeCategory.IsValidEnum(nameof(recipeCategory));

        switch (recipeCategory)
        {
            case RecipeCategory.Meat:
                return new Color(250, 85, 87);
            case RecipeCategory.Fish:
                return new Color(86, 153, 220);
            case RecipeCategory.Vegetarian:
                return new Color(206, 221, 85);
            case RecipeCategory.Vegan:
                return new Color(6, 167, 125);
            case RecipeCategory.Drinks:
                return new Color(175, 234, 224);
            case RecipeCategory.Pastry:
                return new Color(206, 132, 173);
            case RecipeCategory.Dessert:
                return new Color(176, 69, 162);
            case RecipeCategory.Snack:
                return new Color(249, 162, 114);
            case RecipeCategory.Other:
                return new Color(165, 161, 164);
            default:
                throw new NotSupportedException();
        }
    }
}