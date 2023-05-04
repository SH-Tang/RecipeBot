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
using Discord.Common.Providers;

namespace Discord.Common.Utils;

/// <summary>
/// Class containing helper methods for an <see cref="IUser"/>.
/// </summary>
public static class IUserHelper
{
    /// <summary>
    /// Creates a <see cref="UserData"/> based on an <see cref="IUser"/>.
    /// </summary>
    /// <param name="user">The user to create the user data with.</param>
    /// <returns>An <see cref="UserData"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="user"/> is <c>null</c>.</exception>
    public static UserData Create(IUser user)
    {
        user.IsNotNull(nameof(user));

        return new UserData(user.Username, user.GetAvatarUrl());
    }
}