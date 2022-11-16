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
using System.Linq;
using Common.Utils;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Models;
using RecipeBot.Domain.Properties;

namespace RecipeBot.Domain.Factories;

/// <summary>
/// Factory to create instances of <see cref="RecipeTagsModel"/>.
/// </summary>
public class RecipeTagsModelFactory
{
    private readonly ITagModelCharacterLimitProvider limitProvider;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeTagsModelFactory"/>.
    /// </summary>
    /// <param name="limitProvider">The <see cref="ITagModelCharacterLimitProvider"/> to retrieve the character limits from.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="limitProvider"/> is <c>null</c>.</exception>
    public RecipeTagsModelFactory(ITagModelCharacterLimitProvider limitProvider)
    {
        limitProvider.IsNotNull(nameof(limitProvider));
        this.limitProvider = limitProvider;
    }

    /// <summary>
    /// Creates a collection of tags based on the input arguments.
    /// </summary>
    /// <param name="category">The <see cref="RecipeCategory"/> to create the tags for.</param>
    /// <param name="tags">The string representing the unformatted tags.</param>
    /// <returns>A collection of tags.</returns>
    /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/> is an invalid category.</exception>
    /// <exception cref="NotSupportedException">Thrown when <paramref name="category"/> is a valid category,
    /// but not supported.</exception>
    public RecipeTagsModel Create(RecipeCategory category, string? tags)
    {
        category.IsValidEnum(nameof(category));

        var createdTags = new List<string>
        {
            GetValue(category)
        };

        if (!string.IsNullOrWhiteSpace(tags))
        {
            createdTags.AddRange(CreateTagCollection(tags));
        }

        var model = new RecipeTagsModel(createdTags);
        int maximumRecipeTagsLength = limitProvider.MaximumRecipeTagsLength;
        if (model.TotalLength > maximumRecipeTagsLength)
        {
            string exceptionMessage = string.Format(Resources.RecipeTagsModelFactory_RecipeTagsModelTotalLength_must_be_less_or_equal_to_number_of_0_characters,
                                                    maximumRecipeTagsLength);
            throw new ModelCreateException(exceptionMessage);
        }

        return new RecipeTagsModel(createdTags);
    }

    private static IEnumerable<string> CreateTagCollection(string tags)
    {
        string[] splitTags = tags.Split(',');
        return splitTags.Select(t => t.Trim())
                        .Distinct();
    }

    private static string GetValue(RecipeCategory category)
    {
        switch (category)
        {
            case RecipeCategory.Meat:
                return "Meat";
            case RecipeCategory.Fish:
                return "Fish";
            case RecipeCategory.Vegetarian:
                return "Vegetarian";
            case RecipeCategory.Vegan:
                return "Vegan";
            case RecipeCategory.Drinks:
                return "Drinks";
            case RecipeCategory.Pastry:
                return "Pastry";
            case RecipeCategory.Dessert:
                return "Dessert";
            case RecipeCategory.Snack:
                return "Snack";
            case RecipeCategory.Other:
                return "Other";
            default:
                throw new NotSupportedException();
        }
    }
}