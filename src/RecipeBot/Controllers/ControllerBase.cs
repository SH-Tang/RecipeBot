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
    /// <param name="logger">The logger to log with.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is <c>null</c>.</exception>
    protected ControllerBase(ILoggingService logger)
    {
        logger.IsNotNull(nameof(logger));
        this.logger = logger;
    }

    protected ControllerResult<TResult> HandleException<TResult>(Exception e) where TResult : class
    {
        logger.LogError(e);
        return ControllerResult<TResult>.CreateControllerResultWithError(e.Message);
    }
}