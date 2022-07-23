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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;

namespace WeekendBot.Modules;

/// <summary>
/// Interface for describing services related to <see cref="DiscordCommandInformation"/> queries.
/// </summary>
public interface IDiscordCommandInformationService
{
    /// <summary>
    /// Gets an <see cref="Embed"/> that contains the summaries of all available commands.
    /// </summary>
    /// <returns>A <see cref="Embed"/> containing summaries of all available commands.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="commandInfos"/> is <c>null</c>.</exception>
    Task<Embed> GetCommandInfoSummaries(IEnumerable<DiscordCommandInformation> commandInfos);
}