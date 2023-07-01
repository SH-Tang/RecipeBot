using System;
using System.Threading.Tasks;
using Common.Utils;
using Discord.Common.Services;
using RecipeBot.Discord.Controllers;

namespace RecipeBot.Controllers;

/// <summary>
/// Base implementation of controllers.
/// </summary>
public abstract class ControllerBase
{
    private readonly ILoggingService logger;

    /// <summary>
    /// Creates a new instance of <see cref="ControllerBase"/>. 
    /// </summary>
    /// <param name="logger"></param>
    protected ControllerBase(ILoggingService logger)
    {
        logger.IsNotNull(nameof(logger));
        this.logger = logger;
    }

    protected async Task<ControllerResult<TResult>> HandleException<TResult>(Exception e) where TResult : class
    {
        await logger.LogErrorAsync(e);
        return ControllerResult<TResult>.CreateControllerResultWithError(e.Message);
    }
}