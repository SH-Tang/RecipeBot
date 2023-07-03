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
using FluentAssertions;
using RecipeBot.Domain.Repositories.Data;
using Xunit;

namespace RecipeBot.Domain.Test.Repositories.Data;

public class AuthorRepositoryEntityDataTest
{
    [Fact]
    public void Given_data_with_author_id_returns_property_with_author_id_set()
    {
        // Setup
        var fixture = new Fixture();
        var data = new AuthorRepositoryEntityData(fixture.Create<long>(), fixture.Create<ulong>());

        // Call
        bool authorIdSet = data.HasAuthorId;

        // Assert
        authorIdSet.Should().BeTrue();
    }

    [Fact]
    public void Given_data_without_author_id_returns_property_without_author_id_set()
    {
        // Setup
        var fixture = new Fixture();
        var data = new AuthorRepositoryEntityData(fixture.Create<long>());

        // Call
        bool authorIdSet = data.HasAuthorId;

        // Assert
        authorIdSet.Should().BeFalse();
    }
}