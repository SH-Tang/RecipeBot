using System;
using System.ComponentModel;
using Common.Utils;
using RecipeBot.Domain.Data;
using RecipeBot.Persistence.Entities;

namespace RecipeBot.Persistence.Creators;

/// <summary>
/// Creator to create <see cref="PersistentRecipeCategory"/>.
/// </summary>
internal static class PersistentRecipeCategoryCreator
{
    /// <summary>
    /// Creates a <see cref="PersistentRecipeCategory"/> based on its input argument.
    /// </summary>
    /// <param name="category">The <see cref="RecipeCategory"/> to create the <see cref="PersistentRecipeCategory"/> with.</param>
    /// <returns>A <see cref="PersistentRecipeCategory"/>.</returns>
    /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/> is an invalid <see cref="RecipeCategory"/>.</exception>
    /// <exception cref="NotSupportedException">Thrown when <paramref name="category"/> is a valid <see cref="RecipeCategory"/>,
    /// but unsupported.</exception>
    public static PersistentRecipeCategory Create(RecipeCategory category)
    {
        category.IsValidEnum(nameof(category));

        switch (category)
        {
            case RecipeCategory.Meat:
                return PersistentRecipeCategory.Meat;
            case RecipeCategory.Fish:
                return PersistentRecipeCategory.Fish;
            case RecipeCategory.Vegetarian:
                return PersistentRecipeCategory.Vegetarian;
            case RecipeCategory.Vegan:
                return PersistentRecipeCategory.Vegan;
            case RecipeCategory.Drinks:
                return PersistentRecipeCategory.Drinks;
            case RecipeCategory.Pastry:
                return PersistentRecipeCategory.Pastry;
            case RecipeCategory.Dessert:
                return PersistentRecipeCategory.Dessert;
            case RecipeCategory.Snack:
                return PersistentRecipeCategory.Snack;
            case RecipeCategory.Other:
                return PersistentRecipeCategory.Other;
            default:
                throw new NotSupportedException();
        }
    }
}