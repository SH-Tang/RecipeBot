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

namespace RecipeBot.Discord.Controllers;

/// <summary>
/// Class holding the result of a controller.
/// </summary>
/// <typeparam name="TResult">The result to host.</typeparam>
public class ControllerResult<TResult> where TResult : class
{
    /// <summary>
    /// Creates a new instance of <see cref="ControllerResult{TResult}"/> with a valid result.
    /// </summary>
    /// <param name="result">The <typeparamref name="TResult"/> to host.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
    public static ControllerResult<TResult> CreateControllerResultWithValidResult(TResult result)
    {
        result.IsNotNull(nameof(result));

        return new ControllerResult<TResult>(result);
    }

    /// <summary>
    /// Creates a new instance of <see cref="ControllerResult{TResult}"/> with an error message.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    public static ControllerResult<TResult> CreateControllerResultWithError(string errorMessage)
    {
        return new ControllerResult<TResult>(errorMessage);
    }


    /// <summary>
    /// Creates a new instance of <see cref="ControllerResult{TResult}"/> with a valid result.
    /// </summary>
    /// <param name="result">The <typeparamref name="TResult"/> to host.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
    private ControllerResult(TResult result)
    {
        result.IsNotNull(nameof(result));

        HasError = false;
        Result = result;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ControllerResult{TResult}"/> with an error message.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    private ControllerResult(string errorMessage)
    {
        ErrorMessage = errorMessage;
        HasError = true;
    }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Gets the <see cref="TResult"/>.
    /// </summary>
    public TResult? Result { get; }

    /// <summary>
    /// Gets the indicator whether an error has occurred.
    /// </summary>
    public bool HasError { get; init; }
}