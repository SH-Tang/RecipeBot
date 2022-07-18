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

namespace WeekendBot.Core.Options;

/// <summary>
/// Class containing options for string formats.
/// </summary>
public class StringFormatOptions
{
    /// <summary>
    /// Gets the key of the section to retrieve the settings from.
    /// </summary>
    public const string SectionKey = "StringFormatOptions";

    /// <summary>
    /// Gets or sets the numeric format for floating numbers.
    /// </summary>
    public string FloatingNumberFormat { get; set; } = "F";

    /// <summary>
    /// Gets or sets the format to represent <see cref="DateTime"/>.
    /// </summary>
    public string DateTimeFormat { get; set; } = "F";

    /// <summary>
    /// Gets or sets the format to represent <see cref="TimeSpan"/>.
    /// </summary>
    public string TimeSpanFormat { get; set; } = "c";
}