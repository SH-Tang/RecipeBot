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
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Discord;
using FluentAssertions;
using NSubstitute;
using RecipeBot.Controllers;
using RecipeBot.Services;
using RecipeBot.TestUtils;
using Xunit;

namespace RecipeBot.Test.Services;

public class DataEntryCollectionMessageFormattingServiceTest
{
    [Theory]
    [ClassData(typeof(EmptyOrWhiteSpaceStringValueGenerator))]
    public void Constructor_with_invalid_header_throws_exception(string invalidHeader)
    {
        // Setup
        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();

        // Call
        Action call = () => new DataEntryCollectionMessageFormattingService<object>(
            limitProvider, invalidHeader, entry => string.Empty);

        // Assert
        call.Should().Throw<ArgumentException>();
    }

    [Theory]
    [ClassData(typeof(EmptyOrWhiteSpaceStringValueGenerator))]
    public void Creating_message_with_invalid_empty_collection_message_throws_exception(string invalidEmptyMessage)
    {
        // Setup
        var fixture = new Fixture();

        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var service = new DataEntryCollectionMessageFormattingService<object>(limitProvider, fixture.Create<string>(), entry => string.Empty);

        // Call
        Action call = () => service.CreateMessages(Enumerable.Empty<object>(), invalidEmptyMessage);

        // Assert
        call.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Creating_message_with_empty_collection_returns_expected_message()
    {
        // Setup
        var fixture = new Fixture();
        var emptyCollectionMessage = fixture.Create<string>();

        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        var service = new DataEntryCollectionMessageFormattingService<object>(limitProvider, fixture.Create<string>(), entry => string.Empty);

        // Call
        IReadOnlyList<string> messages = service.CreateMessages(Enumerable.Empty<object>(), emptyCollectionMessage);

        // Assert
        messages.Should().HaveCount(1)
                .And.Contain(emptyCollectionMessage);
    }

    [Fact]
    public void Creating_message_with_collection_within_message_character_limits_returns_expected_message()
    {
        // Setup
        var fixture = new Fixture();
        var header = fixture.Create<string>();

        IEnumerable<Entry> entries = fixture.CreateMany<Entry>(3).ToArray();

        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(int.MaxValue);
        var service = new DataEntryCollectionMessageFormattingService<Entry>(limitProvider, header, entry => entry.Property);

        // Call
        IReadOnlyList<string> messages = service.CreateMessages(entries, fixture.Create<string>());

        // Assert
        string expectedMessage = $"{header}{Environment.NewLine}" +
                                 $"{entries.ElementAt(0).Property}{Environment.NewLine}" +
                                 $"{entries.ElementAt(1).Property}{Environment.NewLine}" +
                                 $"{entries.ElementAt(2).Property}{Environment.NewLine}";
        messages.Should().HaveCount(1)
                .And.Contain(Format.Code(expectedMessage));
    }

    [Theory]
    [InlineData(138)] // Exactly intersects with 2 entries
    [InlineData(145)]
    public void Creating_message_with_collection_exceeding_message_character_limits_returns_expected_message(int limit)
    {
        // Setup
        var fixture = new Fixture();
        var header = fixture.Create<string>();

        IEnumerable<Entry> entries = fixture.CreateMany<Entry>(3).ToArray();

        var limitProvider = Substitute.For<IMessageCharacterLimitProvider>();
        limitProvider.MaxMessageLength.Returns(limit);
        var service = new DataEntryCollectionMessageFormattingService<Entry>(limitProvider, header, entry => entry.Property);

        // Call
        IReadOnlyList<string> messages = service.CreateMessages(entries, fixture.Create<string>());

        // Assert
        string expectedMessageOne = $"{header}{Environment.NewLine}" +
                                    $"{entries.ElementAt(0).Property}{Environment.NewLine}" +
                                    $"{entries.ElementAt(1).Property}{Environment.NewLine}";

        string expectedMessageTwo = $"{header}{Environment.NewLine}" +
                                    $"{entries.ElementAt(2).Property}{Environment.NewLine}";

        messages.Should().BeEquivalentTo(new[]
        {
            Format.Code(expectedMessageOne),
            Format.Code(expectedMessageTwo)
        }, options => options.WithStrictOrdering());
    }

    private class Entry
    {
        // Do not remove the setter
        public string Property { get; set; } = null!;
    }
}