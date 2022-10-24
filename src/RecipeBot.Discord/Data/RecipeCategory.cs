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
public enum RecipeCategory
{
    /// <summary>
    /// Represents an appetizer.
    /// </summary>
    [ChoiceDisplay("Appetizer")]
    Appetizer,

    /// <summary>
    /// Represents a main dish.
    /// </summary>
    [ChoiceDisplay("Main dish")]
    MainDish,

    /// <summary>
    /// Represents a dessert.
    /// </summary>
    [ChoiceDisplay("Dessert")]
    Dessert,

    /// <summary>
    /// Represents a side dish.
    /// </summary>
    [ChoiceDisplay("Side dish")]
    Side,

    /// <summary>
    /// Represents a dish that does not belong to any category.
    /// </summary>
    [ChoiceDisplay("Miscellaneous")]
    Miscellaneous
}