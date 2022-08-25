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
using WeekendBot.Utils;

namespace Discord.Common.Options;

/// <summary>
/// Class containing extensions methods for the <see cref="StringFormatOptions"/>.
/// </summary>
public static class StringFormatOptionsExtensions
{
    /// <summary>
    /// Formats a <see cref="double"/>.
    /// </summary>
    /// <param name="options">The <see cref="StringFormatOptions"/> to format with.</param>
    /// <param name="value">The value to format.</param>
    /// <returns>A <see cref="string"/> representing the formatted <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <c>null</c>.</exception>
    public static string Format(this StringFormatOptions options, double value)
    {
        options.IsNotNull(nameof(options));
        return value.ToString(options.FloatingNumberFormat);
    }

    /// <summary>
    /// Formats a <see cref="DateTime"/>.
    /// </summary>
    /// <param name="options">The <see cref="StringFormatOptions"/> to format with.</param>
    /// <param name="value">The value to format.</param>
    /// <returns>A <see cref="string"/> representing the formatted <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <c>null</c>.</exception>
    public static string Format(this StringFormatOptions options, DateTime value)
    {
        options.IsNotNull(nameof(options));
        return value.ToString(options.DateTimeFormat);
    }

    /// <summary>
    /// Formats a <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="options">The <see cref="StringFormatOptions"/> to format with.</param>
    /// <param name="value">The value to format.</param>
    /// <returns>A <see cref="string"/> representing the formatted <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <c>null</c>.</exception>
    public static string Format(this StringFormatOptions options, TimeSpan value)
    {
        options.IsNotNull(nameof(options));
        return value.ToString(options.TimeSpanFormat);
    }
}