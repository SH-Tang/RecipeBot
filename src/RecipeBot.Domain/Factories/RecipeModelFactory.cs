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
using Common.Utils;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Models;
using RecipeBot.Domain.Properties;

namespace RecipeBot.Domain.Factories;

/// <summary>
/// Factory to create instances of <see cref="RecipeModel"/>.
/// </summary>
public class RecipeModelFactory
{
    private readonly AuthorModelFactory authorModelFactory;
    private readonly RecipeFieldModelFactory recipeFieldModelFactory;
    private readonly RecipeTagsModelFactory recipeTagsModelFactory;
    private readonly IRecipeModelCharacterLimitProvider recipeModelCharacterLimitProvider;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeModelFactory"/>.
    /// </summary>
    /// <param name="recipeModelCharacterLimitProvider">The <see cref="IRecipeModelCharacterLimitProvider"/>
    /// to retrieve the character limits from.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="recipeModelCharacterLimitProvider"/>
    /// is <c>null</c>.</exception>
    public RecipeModelFactory(IRecipeModelCharacterLimitProvider recipeModelCharacterLimitProvider)
    {
        recipeModelCharacterLimitProvider.IsNotNull(nameof(recipeModelCharacterLimitProvider));

        this.recipeModelCharacterLimitProvider = recipeModelCharacterLimitProvider;

        authorModelFactory = new AuthorModelFactory(recipeModelCharacterLimitProvider);
        recipeFieldModelFactory = new RecipeFieldModelFactory(recipeModelCharacterLimitProvider);
        recipeTagsModelFactory = new RecipeTagsModelFactory(recipeModelCharacterLimitProvider);
    }

    /// <summary>
    /// Creates a <see cref="RecipeModel"/> based on its input arguments.
    /// </summary>
    /// <param name="recipeData">The <see cref="RecipeData"/> to create the <see cref="RecipeModel"/> with.</param>
    /// <returns>A <see cref="RecipeModel"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="recipeData"/> is <c>null</c>.</exception>
    /// <exception cref="ModelCreateException">Thrown when the model could not be successfully created.</exception>
    public RecipeModel Create(RecipeData recipeData)
    {
        recipeData.IsNotNull(nameof(recipeData));

        int maximumTitleLength = recipeModelCharacterLimitProvider.MaximumTitleLength;
        if (recipeData.RecipeTitle.Length > maximumTitleLength)
        {
            throw new ModelCreateException(string.Format(Resources.Argument_0_must_be_less_or_equal_to_number_of_1_characters,
                                                         nameof(RecipeData.RecipeTitle), maximumTitleLength));
        }

        try
        {
            return CreateRecipe(recipeData);
        }
        catch (ArgumentException e)
        {
            throw new ModelCreateException(e.Message, e);
        }
    }

    /// <summary>
    /// Creates an <see cref="RecipeModel"/> based on its input argument.
    /// </summary>
    /// <param name="recipeData">The <see cref="RecipeData"/> to create the <see cref="RecipeModel"/> with.</param>
    /// <returns>A <see cref="RecipeModel"/>.</returns>
    /// <exception cref="ModelCreateException">Thrown when the <see cref="RecipeModel"/>
    /// could not be successfully created.</exception>
    private RecipeModel CreateRecipe(RecipeData recipeData)
    {
        IEnumerable<RecipeFieldModel> recipeFields = CreateRecipeFields(recipeData);

        RecipeModelMetaData metaData = CreateMetaData(recipeData.AuthorData, recipeData.Tags, recipeData.Category);

        string recipeTitle = recipeData.RecipeTitle;
        RecipeModel recipe = recipeData.ImageUrl == null
                                 ? new RecipeModel(metaData, recipeFields, recipeTitle)
                                 : new RecipeModel(metaData, recipeFields, recipeTitle, recipeData.ImageUrl);

        int maximumRecipeLength = recipeModelCharacterLimitProvider.MaximumRecipeLength;
        if (recipe.TotalLength > maximumRecipeLength)
        {
            throw new ModelCreateException(string.Format(Resources.Argument_0_must_be_less_or_equal_to_number_of_1_characters,
                                                         nameof(recipeData), maximumRecipeLength));
        }

        return recipe;
    }

    /// <summary>
    /// Creates an <see cref="RecipeModelMetaData"/> based on its input arguments.
    /// </summary>
    /// <param name="authorData">The <see cref="AuthorData"/> to create the metadata with.</param>
    /// <param name="tagData">The tags to create the metadata with.</param>
    /// <param name="recipeCategory">The <see cref="RecipeCategory"/> to create the metadata with.</param>
    /// <returns>A <see cref="RecipeModelMetaData"/>.</returns>
    /// <exception cref="ModelCreateException">Thrown when the <see cref="RecipeModelMetaData"/>
    /// could not be successfully created.</exception>
    private RecipeModelMetaData CreateMetaData(AuthorData authorData, string? tagData, RecipeCategory recipeCategory)
    {
        AuthorModel authorModel = authorModelFactory.Create(authorData);
        RecipeTagsModel tagModel= recipeTagsModelFactory.Create(tagData);

        return new RecipeModelMetaData(authorModel, tagModel, recipeCategory);
    }
    
    /// <summary>
    /// Creates a collection of <see cref="RecipeFieldModel"/> based on its input argument.
    /// </summary>
    /// <param name="recipeData">The <see cref="RecipeData"/> to create the collection with.</param>
    /// <returns>A collection of <see cref="RecipeFieldModel"/>.</returns>
    /// <exception cref="ModelCreateException">Thrown when the collection of <see cref="RecipeFieldModel"/>
    /// could not be successfully created.</exception>
    private IEnumerable<RecipeFieldModel> CreateRecipeFields(RecipeData recipeData)
    {
        var recipeFields = new List<RecipeFieldModel>
        {
            recipeFieldModelFactory.Create(Resources.Recipe_FieldName_Ingredients, recipeData.RecipeIngredients),
            recipeFieldModelFactory.Create(Resources.Recipe_FieldName_CookingSteps, recipeData.CookingSteps)
        };

        if (!string.IsNullOrWhiteSpace(recipeData.AdditionalNotes))
        {
            recipeFields.Add(recipeFieldModelFactory.Create(Resources.Recipe_FieldName_AdditionalNotes, recipeData.AdditionalNotes));
        }

        return recipeFields;
    }
}