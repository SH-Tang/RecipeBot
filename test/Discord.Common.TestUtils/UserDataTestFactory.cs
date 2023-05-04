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

using Discord.Common.Providers;

namespace Discord.Common.TestUtils;

/// <summary>
/// Factory to create instances of <see cref="UserData"/> that can be used for testing.
/// </summary>
public static class UserDataTestFactory
{
    /// <summary>
    /// Creates a valid fully configured <see cref="UserData"/>.
    /// </summary>
    /// <returns>A valid fully configured <see cref="UserData"/>.</returns>
    public static UserData CreateFullyConfigured()
    {
        return Create("User");
    }

    /// <summary>
    /// Creates a valid configured <see cref="UserData"/> based on its input arguments.
    /// </summary>
    /// <param name="name">The name to set.</param>
    /// <returns>A valid configured <see cref="UserData"/>.</returns>
    public static UserData Create(string name)
    {
        return new UserData(name, "https://www.recipebot.com");
    }
}