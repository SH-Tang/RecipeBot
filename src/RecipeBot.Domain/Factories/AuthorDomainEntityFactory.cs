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
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Models;
using RecipeBot.Domain.Properties;

namespace RecipeBot.Domain.Factories;

/// <summary>
/// Factory to create instances of <see cref="AuthorModel"/>.
/// </summary>
public class AuthorDomainEntityFactory
{
    private readonly IAuthorDomainEntityCharacterLimitProvider limitProvider;

    /// <summary>
    /// Creates a new instance of <see cref="AuthorDomainEntityFactory"/>.
    /// </summary>
    /// <param name="limitProvider">The provider to retrieve the character limits from.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="limitProvider"/> is <c>null</c>.</exception>
    public AuthorDomainEntityFactory(IAuthorDomainEntityCharacterLimitProvider limitProvider)
    {
        limitProvider.IsNotNull(nameof(limitProvider));
        this.limitProvider = limitProvider;
    }

    /// <summary>
    /// Creates am <see cref="AuthorModel"/> based on its input arguments.
    /// </summary>
    /// <param name="authorData">The <see cref="AuthorData"/> to create the entity with.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="authorData"/> is <c>null</c>.</exception>
    /// <exception cref="DomainEntityCreateException">Thrown when the entity could not be successfully created.</exception>
    public AuthorModel Create(AuthorData authorData)
    {
        authorData.IsNotNull(nameof(authorData));

        int maximumAuthorNameLength = limitProvider.MaximumAuthorNameLength;
        if (authorData.AuthorName.Length > maximumAuthorNameLength)
        {
            throw new DomainEntityCreateException(string.Format(Resources.Argument_0_must_be_less_or_equal_to_number_of_1_characters,
                                                                nameof(AuthorData.AuthorName), maximumAuthorNameLength));
        }

        try
        {
            return new AuthorModel(authorData.AuthorName, authorData.AuthorImageUrl);
        }
        catch (ArgumentException e)
        {
            throw new DomainEntityCreateException(e.Message, e);
        }
    }
}