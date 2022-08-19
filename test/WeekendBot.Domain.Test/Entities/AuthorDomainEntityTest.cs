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
using WeekendBot.Domain.Entities;
using WeekendBot.TestUtils;
using Xunit;

namespace WeekendBot.Domain.Test.Entities;

public class AuthorDomainEntityTest
{
    [Theory]
    [ClassData(typeof(EmptyOrNullStringValueGenerator))]
    public void Entity_with_invalid_name_throws_exception(string invalidAuthorName)
    {
        // Setup
        const string imageUrl = "http://wwww.google.com";

        // Call
        Func<AuthorDomainEntity> call = () => new AuthorDomainEntity(invalidAuthorName, imageUrl);

        // Assert
        Assert.Throws<ArgumentException>(call);
    }

    [Theory]
    [ClassData(typeof(InvalidHttpUrlDataGenerator))]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("     ")]
    public void Entity_with_invalid_image_url_throws_exception(string invalidImageUrl)
    {
        // Setup
        const string authorName = "Author";

        // Call
        Func<AuthorDomainEntity> call = () => new AuthorDomainEntity(authorName, invalidImageUrl);

        // Assert
        Assert.Throws<ArgumentException>(call);
    }

    [Theory]
    [InlineData("authorName")]
    [InlineData("author name")]
    [InlineData("author    name")]
    [InlineData("     authorName")]
    [InlineData("authorName     ")]
    public void Entity_with_valid_data_returns_total_length_of_author_name(string authorName)
    {
        // Setup
        const string imageUrl = "http://wwww.google.com";
        var recipeField = new AuthorDomainEntity(authorName, imageUrl);

        // Call
        int totalLength = recipeField.TotalLength;

        // Assert
        Assert.Equal(authorName.Length, totalLength);
    }
}