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