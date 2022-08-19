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

namespace WeekendBot.Domain.Utils;

/// <summary>
/// Helper class to validate urls.
/// </summary>
public static class UrlValidationHelper
{
    /// <summary>
    /// Validates whether the <paramref name="url"/> is a valid http or https url.
    /// </summary>
    /// <param name="url">The url to validate.</param>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="url"/> is invalid.</exception>
    public static void ValidateHttpUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url) || !IsValidUrl(url))
        {
            throw new ArgumentException($"{nameof(url)} is an invalid http or https url.");
        }
    }

    private static bool IsValidUrl(string url)
    {
        Uri? uriResult;
        bool tryCreateUriResult = Uri.TryCreate(url, UriKind.Absolute, out uriResult);

        return tryCreateUriResult && uriResult != null && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}