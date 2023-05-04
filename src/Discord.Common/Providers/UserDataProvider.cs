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

    public UserDataProvider(DiscordSocketClient client)
    {
        this.client = client;
        client.IsNotNull(nameof(client));
    }

    public async Task<UserData> GetUserDataAsync(ulong userId)
    {
        IUser? user = await client.GetUserAsync(userId);

        return user == null ? UserData.UnknownUser : new UserData(user.Username, user.GetAvatarUrl());
    }
}