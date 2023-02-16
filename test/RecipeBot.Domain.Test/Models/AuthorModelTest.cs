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
using FluentAssertions;
using RecipeBot.Domain.Models;
using RecipeBot.TestUtils;
using Xunit;

namespace RecipeBot.Domain.Test.Models;

public class AuthorModelTest
{
    [Theory]
    [ClassData(typeof(NullOrWhitespacesStringValueGenerator))]
    public void Model_with_invalid_name_throws_exception(string invalidAuthorName)
    {
        // Setup
        const string imageUrl = "http://wwww.google.com";

        // Call
        Action call = () => new AuthorModel(invalidAuthorName, imageUrl);

        // Assert
        call.Should().ThrowExactly<ArgumentException>();
    }

    [Theory]
    [ClassData(typeof(InvalidHttpUrlDataGenerator))]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("     ")]
    public void Model_with_invalid_image_url_throws_exception(string invalidImageUrl)
    {
        // Setup
        const string authorName = "Author";

        // Call
        Action call = () => new AuthorModel(authorName, invalidImageUrl);

        // Assert
        call.Should().ThrowExactly<ArgumentException>();
    }

    [Theory]
    [InlineData("authorName")]
    [InlineData("author name")]
    [InlineData("author    name")]
    [InlineData("     authorName")]
    [InlineData("authorName     ")]
    public void Model_with_valid_data_returns_total_length_of_author_name(string authorName)
    {
        // Setup
        const string imageUrl = "http://wwww.google.com";
        var recipeField = new AuthorModel(authorName, imageUrl);

        // Call
        int totalLength = recipeField.TotalLength;

        // Assert
        totalLength.Should().Be(authorName.Length);
    }
}