using System.Collections.Generic;
using System.Threading.Tasks;
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
        var messages = new[]
        {
            "Recipe One",
            "Recipe Two",
        };

        return Task.FromResult(new ControllerResult<IReadOnlyList<string>>("ErrorMessage"));
    }

    public Task<ControllerResult<IReadOnlyList<string>>> ListAllRecipesAsync(DiscordRecipeCategory category)
    {
        throw new System.NotImplementedException();
    }
}