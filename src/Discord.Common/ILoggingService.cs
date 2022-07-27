// Copyright (C) 2022 Dennis Tang. All rights reserved.
//
// This file is part of WeekendBot.
//
// WeekendBot is free software: you can redistribute it and/or modify
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

using System.Threading.Tasks;

namespace Discord.Common;

/// <summary>
/// Interface describing a service for logging.
/// </summary>
public interface ILoggingService
{
    /// <summary>
    /// Logs an informational message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    Task LogInfoAsync(string message);

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    Task LogWarningAsync(string message);

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    Task LogErrorAsync(string message);

    /// <summary>
    /// Logs an informational message when in debug mode.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    Task LogDebugAsync(string message);
}