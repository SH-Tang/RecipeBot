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
using NSubstitute;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Factories;
using RecipeBot.Domain.Models;
using RecipeBot.TestUtils;
using Xunit;

namespace RecipeBot.Domain.Test.Factories;

public class AuthorModelFactoryTest
{
    [Fact]
    public void Creating_model_with_author_name_with_invalid_character_length_throws_exception()
    {
        // Setup
        const int maximumAuthorNameLength = 10;

        var limitProvider = Substitute.For<IAuthorModelCharacterLimitProvider>();
        limitProvider.MaximumAuthorNameLength.Returns(maximumAuthorNameLength);
        var factory = new AuthorModelFactory(limitProvider);

        var authorName = new string('x', maximumAuthorNameLength + 1);
        var authorData = new AuthorData(authorName, "http://www.google.com");

        // Call
        Action call = () => factory.Create(authorData);

        // Assert
        var expectedMessage = $"AuthorName must be less or equal to {maximumAuthorNameLength} characters.";

        call.Should().Throw<ModelCreateException>()
            .WithMessage(expectedMessage);
    }

    [Theory]
    [ClassData(typeof(InvalidHttpUrlDataGenerator))]
    public void Creating_model_with_invalid_author_image_url_throws_exception(string invalidUrl)
    {
        // Setup
        const int maximumAuthorNameLength = 10;

        var limitProvider = Substitute.For<IAuthorModelCharacterLimitProvider>();
        limitProvider.MaximumAuthorNameLength.Returns(maximumAuthorNameLength);
        var factory = new AuthorModelFactory(limitProvider);

        var authorName = new string('x', maximumAuthorNameLength);
        var authorData = new AuthorData(authorName, invalidUrl);

        // Call
        Action call = () => factory.Create(authorData);

        // Assert
        call.Should().Throw<ModelCreateException>()
            .And.Message.Should().NotStartWith("AuthorName must be less or equal to");
    }

    [Theory]
    [InlineData(8)]
    [InlineData(0)]
    [InlineData(1)]
    public void Creating_model_with_valid_data_returns_model(int authorNameCharacterOffset)
    {
        // Setup
        const int maximumAuthorNameLength = 10;

        var limitProvider = Substitute.For<IAuthorModelCharacterLimitProvider>();
        limitProvider.MaximumAuthorNameLength.Returns(maximumAuthorNameLength);
        var factory = new AuthorModelFactory(limitProvider);

        var authorName = new string('x', maximumAuthorNameLength - authorNameCharacterOffset);
        const string authorImageUrl = "http://www.google.com";
        var authorData = new AuthorData(authorName, authorImageUrl);

        // Call
        AuthorModel model = factory.Create(authorData);

        // Assert
        model.AuthorName.Should().Be(authorName);
        model.AuthorImageUrl.Should().Be(authorImageUrl);
    }
}