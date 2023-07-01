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

namespace Discord.Common.Providers;

/// <summary>
/// Class containing information about the user.
/// </summary>
public class UserData
{
    /// <summary>
    /// Gets the default user.
    /// </summary>
    internal static readonly UserData UnknownUser = new UserData("Unknown user", null);

    /// <summary>
    /// Creates a new instance of <see cref="UserData"/>.
    /// </summary>
    /// <param name="username">The name of the user.</param>
    /// <param name="userImageUrl">The image <see cref="Uri"/> of the user.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="username"/> is <c>null</c>, empty or consists of whitespace only.</exception>
    internal UserData(string username, string? userImageUrl)
    {
        username.IsNotNullOrWhiteSpaces(nameof(username));

        Username = username;
        UserImageUrl = userImageUrl;
    }

    /// <summary>
    /// Gets the name of the user.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the image url of the user.
    /// </summary>
    public string? UserImageUrl { get; }
}