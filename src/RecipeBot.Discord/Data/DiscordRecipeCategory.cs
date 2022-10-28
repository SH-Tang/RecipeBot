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

using Discord.Interactions;

namespace RecipeBot.Discord.Data;

/// <summary>
/// Enum to indicate which category the recipe belongs to.
/// </summary>
public enum DiscordRecipeCategory
{
    /// <summary>
    /// Represents a recipe based on meat.
    /// </summary>
    [ChoiceDisplay("Meat")]
    Meat,

    /// <summary>
    /// Represents a recipe based on fish.
    /// </summary>
    [ChoiceDisplay("Fish")]
    Fish,

    /// <summary>
    /// Represents a recipe for a vegetarian dish.
    /// </summary>
    [ChoiceDisplay("Vegetarian")]
    Vegetarian,

    /// <summary>
    /// Represents a recipe for a vegan dish.
    /// </summary>
    [ChoiceDisplay("Vegan")]
    Vegan,

    /// <summary>
    /// Represents a recipe for a drink.
    /// </summary>
    [ChoiceDisplay("Drinks")]
    Drinks,

    /// <summary>
    /// Represents a recipe for a pastry dish.
    /// </summary>
    [ChoiceDisplay("Pastry")]
    Pastry,

    /// <summary>
    /// Represents a recipe for a dessert.
    /// </summary>
    [ChoiceDisplay("Dessert")]
    Dessert,

    /// <summary>
    /// Represents a recipe for a snack.
    /// </summary>
    [ChoiceDisplay("Snack")]
    Snack,

    /// <summary>
    /// Represents a dish that does not belong to any category.
    /// </summary>
    [ChoiceDisplay("Other")]
    Other
}