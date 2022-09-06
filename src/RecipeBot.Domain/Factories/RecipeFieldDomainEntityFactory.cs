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
using Common.Utils;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Models;
using RecipeBot.Domain.Properties;

namespace RecipeBot.Domain.Factories;

/// <summary>
/// Factory to create instances of <see cref="RecipeFieldModel"/>.
/// </summary>
public class RecipeFieldDomainEntityFactory
{
    private readonly IRecipeFieldModelCharacterLimitProvider limitProvider;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeFieldModel"/>.
    /// </summary>
    /// <param name="limitProvider">The provider to retrieve the character limits from.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="limitProvider"/> is <c>null</c>.</exception>         
    public RecipeFieldDomainEntityFactory(IRecipeFieldModelCharacterLimitProvider limitProvider)
    {
        limitProvider.IsNotNull(nameof(limitProvider));

        this.limitProvider = limitProvider;
    }

    /// <summary>
    /// Creates a <see cref="RecipeFieldModel"/> based on its input arguments.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <param name="fieldData">The data of the field.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    /// <exception cref="DomainEntityCreateException">Thrown when the entity could not be successfully created.</exception>
    public RecipeFieldModel Create(string fieldName, string fieldData)
    {
        fieldName.IsNotNull(nameof(fieldName));
        fieldData.IsNotNull(nameof(fieldData));

        int maximumFieldNameLength = limitProvider.MaximumFieldNameLength;
        if (fieldName.Length > maximumFieldNameLength)
        {
            throw new DomainEntityCreateException(CreateInvalidCharacterLengthExceptionMessage(nameof(fieldName), maximumFieldNameLength));
        }

        int maximumFieldDataLength = limitProvider.MaximumFieldDataLength;
        if (fieldData.Length > maximumFieldDataLength)
        {
            throw new DomainEntityCreateException(CreateInvalidCharacterLengthExceptionMessage(nameof(fieldData), maximumFieldDataLength));
        }

        try
        {
            return new RecipeFieldModel(fieldName, fieldData);
        }
        catch (ArgumentException e)
        {
            throw new DomainEntityCreateException(e.Message);
        }
    }

    private static string CreateInvalidCharacterLengthExceptionMessage(string parameterName, int maximumCharacterLength)
    {
        return string.Format(Resources.Argument_0_must_be_less_or_equal_to_number_of_1_characters, parameterName, maximumCharacterLength);
    }
}