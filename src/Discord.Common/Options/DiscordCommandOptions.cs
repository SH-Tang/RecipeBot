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

namespace Discord.Common.Options;

/// <summary>
/// Class containing options for Discord commands.
/// </summary>
public class DiscordCommandOptions
{
    /// <summary>
    /// Gets the key of the section to retrieve the settings from.
    /// </summary>
    public const string SectionKey = "CommandOptions";

    /// <summary>
    /// Gets or sets the id for the test Discord server.
    /// </summary>
    public ulong? TestGuildId { get; set; }
}