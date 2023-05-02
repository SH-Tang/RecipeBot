using System.Threading.Tasks;

namespace Discord.Common.Providers;

/// <summary>
/// Interface for describing providers of user data.
/// </summary>
public interface IUserDataProvider
{
    /// <summary>
    /// Gets the <see cref="UserData"/> based on its input arguments.
    /// </summary>
    /// <param name="userId">The user id to retrieve the user data for.</param>
    /// <returns>The <see cref="UserData"/>.</returns>
    Task<UserData> GetUserDataAsync(ulong userId);
}