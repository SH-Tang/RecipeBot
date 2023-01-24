using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Utils;
using RecipeBot.Discord.Controllers;
using RecipeBot.Discord.Data;

namespace RecipeBot.Controllers;

/// <summary>
/// A concrete implementation of the <see cref="IRecipeCollectionController"/>.
/// </summary>
public class RecipeCollectionControllerMock : IRecipeCollectionController
{
    public Task<ControllerResult<IReadOnlyList<string>>> ListAllRecipesAsync()
    {
        return Task.FromResult(new ControllerResult<IReadOnlyList<string>>("Not implemented yet"));
    }

    public Task<ControllerResult<IReadOnlyList<string>>> ListAllRecipesAsync(DiscordRecipeCategory category)
    {
        category.IsValidEnum(nameof(category));

        return Task.FromResult(new ControllerResult<IReadOnlyList<string>>("Not implemented yet"));
    }
}