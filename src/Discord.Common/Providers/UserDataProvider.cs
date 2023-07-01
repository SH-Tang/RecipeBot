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
using System.Threading.Tasks;
using Common.Utils;
using Discord.WebSocket;

namespace Discord.Common.Providers;

/// <summary>
/// Class for providing user data.
/// </summary>
public class UserDataProvider : IUserDataProvider
{
    private readonly DiscordSocketClient client;

    /// <summary>
    /// Creates a <see cref="UserDataProvider"/>.
    /// </summary>
    /// <param name="client">The <see cref="DiscordSocketClient"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="client"/> is <c>null</c>.</exception>
    public UserDataProvider(DiscordSocketClient client)
    {
        client.IsNotNull(nameof(client));

        this.client = client;
    }

    public async Task<UserData> GetUserDataAsync(ulong userId)
    {
        IUser? user = await client.GetUserAsync(userId);

        if (user == null)
        {
            return UserData.UnknownUser;
        }

        string displayName = user.GlobalName ?? user.Username;
        return new UserData(displayName, user.GetAvatarUrl());
    }
}