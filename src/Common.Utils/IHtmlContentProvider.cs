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
using System.Threading.Tasks;

namespace Common.Utils;

/// <summary>
/// Interface for describing a html content provider.
/// </summary>
public interface IHtmlContentProvider
{
    /// <summary>
    /// Gets the html content based on an url.
    /// </summary>
    /// <param name="url">The url to get the html content from.</param>
    /// <returns>A string containing the html content.</returns>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="url"/> is invalid.</exception>
    Task<string> GetHtmlContent(string url);
}