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
using Discord.Common.Providers;
using FluentAssertions;
using RecipeBot.TestUtils;
using Xunit;

namespace Discord.Common.Test.Providers;

public class UserDataTest
{
    [Theory]
    [ClassData(typeof(NullOrWhitespacesStringValueGenerator))]
    public void Data_with_invalid_username_value_throws_exception(string invalidUsername)
    {
        // Setup
        const string authorImageUrl = "Url";

        // Call
        Action call = () => new UserData(invalidUsername, authorImageUrl);

        // Assert
        call.Should().ThrowExactly<ArgumentException>();
    }

    [Theory]
    [ClassData(typeof(NullOrWhitespacesStringValueGenerator))]
    public void AuthorData_with_invalid_user_image_url_throws_exception(string invalidUserImageUrl)
    {
        // Setup
        const string username = "User name";

        // Call
        Action call = () => new UserData(username, invalidUserImageUrl);

        // Assert
        call.Should().ThrowExactly<ArgumentException>();
    }
}