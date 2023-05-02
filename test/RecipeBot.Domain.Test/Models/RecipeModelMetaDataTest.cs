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
using System.ComponentModel;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Models;
using Xunit;

namespace RecipeBot.Domain.Test.Models;

public class RecipeModelMetaDataTest
{
    [Fact]
    public void Creating_metadata_with_invalid_category_throws_exception()
    {
        // Setup
        var fixture = new Fixture();
        var authorId = fixture.Create<ulong>();
        AuthorModel author = fixture.Build<AuthorModel>()
                                    .FromFactory<string>(authorName => new AuthorModel(authorName, "http://www.recipeBotImage.com"))
                                    .Create();
        RecipeTagsModel tags = fixture.Build<RecipeTagsModel>()
                                      .FromFactory(() => new RecipeTagsModel(Enumerable.Empty<string>()))
                                      .Create();

        const RecipeCategory invalidCategory = (RecipeCategory)(-1);

        // Call
        Action call = () => new RecipeModelMetaData(authorId, tags, invalidCategory);

        // Assert
        call.Should().Throw<InvalidEnumArgumentException>();
    }
}