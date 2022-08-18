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
using WeekendBot.Domain.Utils;
using WeekendBot.Utils;

namespace WeekendBot.Domain.Entities;

/// <summary>
/// Entity containing author data.
/// </summary>
public class AuthorDomainEntity : ITotalCharacterLength
{
    /// <summary>
    /// Creates a new instance of <see cref="AuthorDomainEntity"/>.
    /// </summary>
    /// <param name="authorName">The name of the author.</param>
    /// <param name="authorImageUrl">The image url of the author.</param>
    /// <exception cref="ArgumentException">Thrown when:
    /// <list type="bullet">
    /// <item>The <paramref name="authorName"/> is <c>null</c>, empty or consists of whitespace only.</item>
    /// <item>The <paramref name="authorImageUrl"/> is an invalid url.</item>
    /// </list></exception>
    internal AuthorDomainEntity(string authorName, string authorImageUrl)
    {
        authorName.IsNotNullOrWhiteSpaces(nameof(authorName));
        UrlValidationHelper.ValidateHttpUrl(authorImageUrl);

        AuthorName = authorName;
        AuthorImageUrl = authorImageUrl;
    }

    /// <summary>
    /// Gets the author name.
    /// </summary>
    public string AuthorName { get; }

    /// <summary>
    /// Gets the image url of the author.
    /// </summary>
    public string AuthorImageUrl { get; }

    public int TotalLength => AuthorName.Length;
}