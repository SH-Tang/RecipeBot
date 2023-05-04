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

using AutoFixture;
using Discord.Common.Providers;
using Discord.Common.Utils;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Discord.Common.Test.Utils;

public class IUserHelperTest
{
    [Fact]
    public void Given_Discord_user_when_creating_return_expected_user_data()
    {
        // Setup
        var fixture = new Fixture();
        var userName = fixture.Create<string>();
        var userImageUrl = fixture.Create<string>();

        var user = Substitute.For<IUser>();
        user.Username.Returns(userName);
        user.GetAvatarUrl().Returns(userImageUrl);

        // Call
        UserData userData = IUserHelper.Create(user);

        // Assert
        userData.Username.Should().Be(userName);
        userData.UserImageUrl.Should().Be(userImageUrl);
    }
}
