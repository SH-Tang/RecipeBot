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
public class RecipeDomainEntityFactory
{
    private readonly IRecipeModelCharacterLimitProvider recipeModelCharacterLimitProvider;
    private readonly AuthorModelFactory authorModelFactory;
    private readonly RecipeFieldDomainEntityFactory recipeFieldDomainEntityFactory;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeDomainEntityFactory"/>.
    /// </summary>
    /// <param name="recipeModelCharacterLimitProvider">The <see cref="IRecipeModelCharacterLimitProvider"/>
    /// to retrieve the character limits from.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="recipeModelCharacterLimitProvider"/>
    /// is <c>null</c>.</exception>
    public RecipeDomainEntityFactory(IRecipeModelCharacterLimitProvider recipeModelCharacterLimitProvider)
    {
        recipeModelCharacterLimitProvider.IsNotNull(nameof(recipeModelCharacterLimitProvider));

        this.recipeModelCharacterLimitProvider = recipeModelCharacterLimitProvider;

        authorModelFactory = new AuthorModelFactory(recipeModelCharacterLimitProvider);
        recipeFieldDomainEntityFactory = new RecipeFieldDomainEntityFactory(recipeModelCharacterLimitProvider);
    }

    /// <summary>
    /// Creates a <see cref="RecipeModel"/> based on its input arguments.
    /// </summary>
    /// <param name="recipeData">The <see cref="RecipeData"/> to create the <see cref="RecipeModel"/> with.</param>
    /// <returns>A <see cref="RecipeModel"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="recipeData"/> is <c>null</c>.</exception>
    /// <exception cref="ModelCreateException">Thrown when the domain entity could not be successfully created.</exception>
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
            return CreateRecipeDomainEntity(recipeData);
        }
        catch (ArgumentException e)
        {
            throw new ModelCreateException(e.Message, e);
        }
    }

    private RecipeModel CreateRecipeDomainEntity(RecipeData recipeData)
    {
        AuthorModel authorModel = CreateAuthorDomainEntity(recipeData.AuthorData);
        IEnumerable<RecipeFieldModel> fieldEntities = CreateRecipeFieldDomainEntities(recipeData);

        string recipeTitle = recipeData.RecipeTitle;
        RecipeModel entity = recipeData.ImageUrl == null
                                        ? new RecipeModel(authorModel, fieldEntities, recipeTitle)
                                        : new RecipeModel(authorModel, fieldEntities, recipeTitle, recipeData.ImageUrl);

        int maximumRecipeLength = recipeModelCharacterLimitProvider.MaximumRecipeLength;
        if (entity.TotalLength > maximumRecipeLength)
        {
            throw new ModelCreateException(string.Format(Resources.Argument_0_must_be_less_or_equal_to_number_of_1_characters,
                                                                nameof(recipeData), maximumRecipeLength));
        }

        return entity;
    }

    private AuthorModel CreateAuthorDomainEntity(AuthorData authorData)
    {
        return authorModelFactory.Create(authorData);
    }

    private IEnumerable<RecipeFieldModel> CreateRecipeFieldDomainEntities(RecipeData recipeData)
    {
        var domainEntities = new List<RecipeFieldModel>
        {
            recipeFieldDomainEntityFactory.Create(Resources.RecipeDomainEntity_FieldName_Ingredients, recipeData.RecipeIngredients),
            recipeFieldDomainEntityFactory.Create(Resources.RecipeDomainEntity_FieldName_CookingSteps, recipeData.CookingSteps)
        };

        if (!string.IsNullOrWhiteSpace(recipeData.AdditionalNotes))
        {
            domainEntities.Add(recipeFieldDomainEntityFactory.Create(Resources.RecipeDomainEntity_FieldName_AdditionalNotes, recipeData.AdditionalNotes));
        }

        return domainEntities;
    }
}