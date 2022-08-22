// Copyright (C) 2022 Dennis Tang. All rights reserved.
//
// This file is part of WeekendBot.
//
// WeekendBot is free software: you can redistribute it and/or modify
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
using WeekendBot.Domain.Data;
using WeekendBot.Domain.Entities;
using WeekendBot.Domain.Exceptions;
using WeekendBot.Domain.Properties;
using WeekendBot.Utils;

namespace WeekendBot.Domain.Factories;

/// <summary>
/// Factory to create instances of <see cref="RecipeDomainEntity"/>.
/// </summary>
public class RecipeDomainEntityFactory
{
    private readonly IRecipeDomainEntityCharacterLimitProvider recipeDomainEntityCharacterLimitProvider;
    private readonly AuthorDomainEntityFactory authorDomainEntityFactory;
    private readonly RecipeFieldDomainEntityFactory recipeFieldDomainEntityFactory;

    public RecipeDomainEntityFactory(IRecipeDomainEntityCharacterLimitProvider recipeDomainEntityCharacterLimitProvider,
                                     IAuthorDomainEntityCharacterLimitProvider authorDomainEntityCharacterLimitProvider,
                                     IRecipeFieldDomainEntityCharacterLimitProvider recipeFieldDomainEntityCharacterLimitProvider)
    {
        recipeDomainEntityCharacterLimitProvider.IsNotNull(nameof(recipeDomainEntityCharacterLimitProvider));
        authorDomainEntityCharacterLimitProvider.IsNotNull(nameof(authorDomainEntityCharacterLimitProvider));
        recipeFieldDomainEntityCharacterLimitProvider.IsNotNull(nameof(recipeFieldDomainEntityCharacterLimitProvider));

        this.recipeDomainEntityCharacterLimitProvider = recipeDomainEntityCharacterLimitProvider;

        authorDomainEntityFactory = new AuthorDomainEntityFactory(authorDomainEntityCharacterLimitProvider);
        recipeFieldDomainEntityFactory = new RecipeFieldDomainEntityFactory(recipeFieldDomainEntityCharacterLimitProvider);
    }

    /// <summary>
    /// Creates an <see cref="RecipeDomainEntity"/> based on its input arguments.
    /// </summary>
    /// <param name="recipeData">The <see cref="RecipeData"/> to create the <see cref="RecipeDomainEntity"/> with.</param>
    /// <returns>A <see cref="RecipeDomainEntity"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="recipeData"/> is <c>null</c>.</exception>
    /// <exception cref="DomainEntityCreateException">Thrown when the domain entity could not be successfully created.</exception>
    public RecipeDomainEntity Create(RecipeData recipeData)
    {
        recipeData.IsNotNull(nameof(recipeData));

        int maximumTitleLength = recipeDomainEntityCharacterLimitProvider.MaximumTitleLength;
        if (recipeData.RecipeTitle.Length > maximumTitleLength)
        {
            throw new DomainEntityCreateException(string.Format(Resources.Argument_0_must_be_less_or_equal_to_number_of_1_characters,
                                                                nameof(RecipeData.RecipeTitle), maximumTitleLength));
        }

        
        return CreateRecipeDomainEntity(recipeData);
    }

    private RecipeDomainEntity CreateRecipeDomainEntity(RecipeData recipeData)
    {
        AuthorDomainEntity authorDomainEntity = CreateAuthorDomainEntity(recipeData.AuthorData);
        IEnumerable<RecipeFieldDomainEntity> fieldEntities = CreateRecipeFieldDomainEntities(recipeData);

        string recipeTitle = recipeData.RecipeTitle;
        RecipeDomainEntity entity = recipeData.ImageUrl == null
                                        ? new RecipeDomainEntity(authorDomainEntity, fieldEntities, recipeTitle)
                                        : new RecipeDomainEntity(authorDomainEntity, fieldEntities, recipeTitle, recipeData.ImageUrl);

        int maximumRecipeLength = recipeDomainEntityCharacterLimitProvider.MaximumRecipeLength;
        if (entity.TotalLength > maximumRecipeLength)
        {
            throw new DomainEntityCreateException(string.Format(Resources.Argument_0_must_be_less_or_equal_to_number_of_1_characters,
                                                                nameof(recipeData), maximumRecipeLength));
        }

        return entity;
    }

    private AuthorDomainEntity CreateAuthorDomainEntity(AuthorData authorData)
    {
        return authorDomainEntityFactory.Create(authorData);
    }

    private IEnumerable<RecipeFieldDomainEntity> CreateRecipeFieldDomainEntities(RecipeData recipeData)
    {
        var domainEntities = new List<RecipeFieldDomainEntity>
        {
            recipeFieldDomainEntityFactory.Create("Ingredients", recipeData.RecipeIngredients),
            recipeFieldDomainEntityFactory.Create("Cooking steps", recipeData.CookingSteps)
        };

        if (!string.IsNullOrWhiteSpace(recipeData.AdditionalNotes))
        {
            domainEntities.Add(recipeFieldDomainEntityFactory.Create("Additional notes", recipeData.AdditionalNotes));
        }

        return domainEntities;
    }
}