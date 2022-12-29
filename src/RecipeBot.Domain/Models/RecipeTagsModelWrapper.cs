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
using System.ComponentModel;
using Common.Utils;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Properties;

namespace RecipeBot.Domain.Models;

/// <summary>
/// Wrapper for the <see cref="RecipeTagsModel"/>.
/// </summary>
public class RecipeTagsModelWrapper
{
    private readonly RecipeTagsModel model;
    private readonly IEnumerable<string> allTags;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeTagsModelWrapper"/>.
    /// </summary>
    /// <param name="model">The <see cref="RecipeTagsModel"/> to wrap.</param>
    /// <param name="category">The <see cref="RecipeCategory"/> the tags belong to.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/> is an invalid <see cref="RecipeCategory"/>.</exception>
    /// <exception cref="NotSupportedException">Thrown when <paramref name="category"/> is a valid <see cref="RecipeCategory"/>,
    /// but not supported.</exception>
    internal RecipeTagsModelWrapper(RecipeTagsModel model, RecipeCategory category)
    {
        model.IsNotNull(nameof(model));
        category.IsValidEnum(nameof(category));

        var tagCollection = new List<string>
        {
            GetValue(category)
        };
        tagCollection.AddRange(model.Tags);
        allTags = tagCollection;

        this.model = model;
    }

    /// <summary>
    /// Gets the collection of tags.
    /// </summary>
    public IEnumerable<string> Tags => model.Tags;

    /// <summary>
    /// Gets the total character length of the model.
    /// </summary>
    public int TotalLength => ToString().Length;

    public override string ToString()
    {
        return string.Join(", ", allTags);
    }

    private static string GetValue(RecipeCategory category)
    {
        switch (category)
        {
            case RecipeCategory.Meat:
                return Resources.RecipeCategoryMeat_DisplayName;
            case RecipeCategory.Fish:
                return Resources.RecipeCategoryFish_DisplayName;
            case RecipeCategory.Vegetarian:
                return Resources.RecipeCategoryVegetarian_DisplayName;
            case RecipeCategory.Vegan:
                return Resources.RecipeCategoryVegan_DisplayName;
            case RecipeCategory.Drinks:
                return Resources.RecipeCategoryDrinks_DisplayName;
            case RecipeCategory.Pastry:
                return Resources.RecipeCategoryPastry_DisplayName;
            case RecipeCategory.Dessert:
                return Resources.RecipeCategoryDessert_DisplayName;
            case RecipeCategory.Snack:
                return Resources.RecipeCategorySnack_DisplayName;
            case RecipeCategory.Other:
                return Resources.RecipeCategoryOther_DisplayName;
            default:
                throw new NotSupportedException();
        }
    }
}