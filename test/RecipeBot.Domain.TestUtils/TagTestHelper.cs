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
using System.Linq;
using System.Text.RegularExpressions;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Models;

namespace RecipeBot.Domain.TestUtils;

/// <summary>
/// Helper class which can be used for asserting tags in tests.
/// </summary>
public static class TagTestHelper
{
    /// <summary>
    /// Gets the mapping of the string representations against each <see cref="RecipeCategory"/>.
    /// </summary>

    public static IReadOnlyDictionary<RecipeCategory, string> CategoryMapping =>
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

    /// <summary>
    /// Gets the collection of parsed tags based on its input arguments.
    /// </summary>
    /// <param name="tagData">The string to parse the tags from.</param>
    /// <returns>A collection of parsed tags.</returns>
    public static IEnumerable<string> GetParsedTags(string tagData)
    {
        return tagData.Split(',')
                      .Select(t => Regex.Replace(t, @"\s+", "").ToLower())
                      .Distinct();
    }

    /// <summary>
    /// Gets the total character length based on the input arguments.
    /// </summary>
    /// <param name="category">The <see cref="RecipeCategory"/>.</param>
    /// <param name="model">The <see cref="RecipeTagsModel"/>.</param>
    /// <returns>Gets the total length of the tags.</returns>
    public static int GetTotalTagsLength(RecipeCategory category, RecipeTagsModel model)
    {
        IEnumerable<string> tags = GetTags(category, model);
        string expectedStringRepresentation = string.Join(", ", tags);

        return expectedStringRepresentation.Length;
    }

    private static IEnumerable<string> GetTags(RecipeCategory category, RecipeTagsModel model)
    {
        var tags = new List<string>
        {
            CategoryMapping[category]
        };
        tags.AddRange(model.Tags);

        return tags;
    }
}