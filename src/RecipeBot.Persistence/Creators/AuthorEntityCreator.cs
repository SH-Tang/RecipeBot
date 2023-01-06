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
using RecipeBot.Domain.Models;
using RecipeBot.Persistence.Entities;

namespace RecipeBot.Persistence.Creators;

/// <summary>
/// Creator to create instances of <see cref="AuthorEntity"/>.
/// </summary>
internal static class AuthorEntityCreator
{
    /// <summary>
    /// Creates an <see cref="AuthorEntity"/> based on its input arguments.
    /// </summary>
    /// <param name="model">The <see cref="AuthorModel"/> to create the
    /// entity with.</param>
    /// <returns>An <see cref="AuthorEntity"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
    public static AuthorEntity Create(AuthorModel model)
    {
        model.IsNotNull(nameof(model));

        return new AuthorEntity
        {
            AuthorName = model.AuthorName,
            AuthorImageUrl = model.AuthorImageUrl
        };
    }
}