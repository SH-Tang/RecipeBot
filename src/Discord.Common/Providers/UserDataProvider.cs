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