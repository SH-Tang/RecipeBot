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
using Xunit;

namespace WeekendBot.Services.Test;

public class AuthorDataTest
{
    [Theory]
    [MemberData(nameof(GetInvalidStringValues))]
    public void AuthorData_with_invalid_author_value_throws_exception(string invalidAuthorName)
    {
        // Setup
        const string authorImageUrl = "Url";

        // Call
        Func<AuthorData> call = () => new AuthorData(invalidAuthorName, authorImageUrl);

        // Assert
        Assert.Throws<ArgumentException>(call);
    }

    [Theory]
    [MemberData(nameof(GetInvalidStringValues))]
    public void AuthorData_with_invalid_author_image_url_throws_exception(string invalidAuthorImageUrl)
    {
        // Setup
        const string authorName = "Author name";

        // Call
        Func<AuthorData> call = () => new AuthorData(authorName, invalidAuthorImageUrl);

        // Assert
        Assert.Throws<ArgumentException>(call);
    }

    private static IEnumerable<object[]> GetInvalidStringValues()
    {
        yield return new object[]
        {
            string.Empty
        };

        yield return new object[]
        {
            "    "
        };

        yield return new object[]
        {
            null
        };
    }
}