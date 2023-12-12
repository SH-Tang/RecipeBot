using System.Threading.Tasks;
using Discord;

namespace RecipeBot.Discord.Controllers;

public interface IWebRecipeController
{
    Task<ControllerResult<Embed>> ParseRecipe(string webRecipeUrl, IUser user, string alternativeTitle);
}