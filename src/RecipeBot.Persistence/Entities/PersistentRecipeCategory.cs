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

namespace RecipeBot.Persistence.Entities;

public enum PersistentRecipeCategory
{
    /// <summary>
    /// Represents a dish that does not belong to any category.
    /// </summary>
    Other = 1,

    /// <summary>
    /// Represents a recipe based on fish.
    /// </summary>
    Fish = 2,

    /// <summary>
    /// Represents a recipe based on meat.
    /// </summary>
    Meat = 3,

    /// <summary>
    /// Represents a recipe for a drink.
    /// </summary>
    Drinks = 4,

    /// <summary>
    /// Represents a recipe for a pastry dish.
    /// </summary>
    Pastry = 5,

    /// <summary>
    /// Represents a recipe for a dessert.
    /// </summary>
    Dessert = 6,

    /// <summary>
    /// Represents a recipe for a snack.
    /// </summary>
    Snack = 7,

    /// <summary>
    /// Represents a recipe for a vegetarian dish.
    /// </summary>
    Vegetarian = 8,

    /// <summary>
    /// Represents a recipe for a vegan dish.
    /// </summary>
    Vegan = 9
}