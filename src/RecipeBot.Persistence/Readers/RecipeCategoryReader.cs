using System;
using System.ComponentModel;
using Common.Utils;
using RecipeBot.Domain.Data;
using RecipeBot.Persistence.Entities;

namespace RecipeBot.Persistence.Readers;

/// <summary>
/// Reader to get values of <see cref="RecipeCategory"/> from the persistence medium.
/// </summary>
internal static class RecipeCategoryReader
{
    /// <summary>
    /// Reads a <see cref="RecipeCategory"/> based on its input argument.
    /// </summary>
    /// <param name="category">The <see cref="PersistentRecipeCategory"/> to read from.</param>
    /// <returns>A <see cref="RecipeCategory"/>.</returns>
    /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/> is an invalid <see cref="PersistentRecipeCategory"/>.</exception>
    /// <exception cref="NotSupportedException">Thrown when <paramref name="category"/> is a valid <see cref="PersistentRecipeCategory"/>,
    /// but unsupported.</exception>
    public static RecipeCategory Read(PersistentRecipeCategory category)
    {
        category.IsValidEnum(nameof(category));

        switch (category)
        {
            case PersistentRecipeCategory.Meat:
                return RecipeCategory.Meat;
            case PersistentRecipeCategory.Fish:
                return RecipeCategory.Fish;
            case PersistentRecipeCategory.Vegetarian:
                return RecipeCategory.Vegetarian;
            case PersistentRecipeCategory.Vegan:
                return RecipeCategory.Vegan;
            case PersistentRecipeCategory.Drinks:
                return RecipeCategory.Drinks;
            case PersistentRecipeCategory.Pastry:
                return RecipeCategory.Pastry;
            case PersistentRecipeCategory.Dessert:
                return RecipeCategory.Dessert;
            case PersistentRecipeCategory.Snack:
                return RecipeCategory.Snack;
            case PersistentRecipeCategory.Other:
                return RecipeCategory.Other;
            default:
                throw new NotSupportedException();
        }
    }
}