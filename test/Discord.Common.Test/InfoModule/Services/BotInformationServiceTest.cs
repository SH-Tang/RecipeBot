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
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Discord.Common.InfoModule.Data;
using Discord.Common.InfoModule.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Discord.Common.Test.InfoModule.Services;

public class BotInformationServiceTest
{
    [Theory]
    [MemberData(nameof(GetInfoOptions))]
    public void GetCommandInfoSummaries_WithOptions_ReturnsEmbedWithExpectedMetaData(
        BotInformation botInformation)
    {
        // Setup
        var options = Substitute.For<IOptions<BotInformation>>();
        options.Value.Returns(botInformation);

        var service = new BotInformationService(options);

        // Call
        Embed result = service.GetCommandInfoSummaries(Enumerable.Empty<DiscordCommandInfo>());

        // Assert
        result.Color.Should().Be(Color.Blue);

        string expectedTitle = botInformation.BotName == null
            ? "Available commands"
            : $"Available commands for {botInformation.BotName}";
        result.Title.Should().Be(expectedTitle);
        result.Url.Should().Be(botInformation.BotInformationUrl);

        EmbedAuthor? resultAuthor = result.Author;
        resultAuthor.Should().NotBeNull();

        EmbedAuthor embedAuthor = result.Author!.Value;
        AuthorInformation? authorInformation = botInformation.AuthorInformation;
        embedAuthor.Name.Should().Be(authorInformation?.AuthorName);
        embedAuthor.Url.Should().Be(authorInformation?.AuthorUrl);
        embedAuthor.IconUrl.Should().Be(authorInformation?.AuthorAvatarUrl);
    }

    [Fact]
    public void GetCommandInfoSummaries_WithCommandInfos_ReturnsExpectedEmbedFields()
    {
        // Setup
        var options = Substitute.For<IOptions<BotInformation>>();
        options.Value.Returns(new BotInformation());

        var service = new BotInformationService(options);

        var infoWithoutSummary = new DiscordCommandInfo("Command1");
        var infoWithSummary = new DiscordCommandInfo("Command2")
        {
            Summary = "Summary"
        };
        IEnumerable<DiscordCommandInfo> commandInfos = new[]
        {
            infoWithoutSummary,
            infoWithSummary
        };

        // Call
        Embed result = service.GetCommandInfoSummaries(commandInfos);

        // Assert
        ImmutableArray<EmbedField> embedFields = result.Fields;
        embedFields.Should().HaveCount(2)
                   .And.SatisfyRespectively(
                       firstField =>
                       {
                           firstField.Name.Should().Be(commandInfos.ElementAt(0).Name);
                           firstField.Value.Should().Be($"No description available.{Environment.NewLine}");
                       },
                       secondField =>
                       {
                           secondField.Name.Should().Be(commandInfos.ElementAt(1).Name);
                           secondField.Value.Should().Be(commandInfos.ElementAt(1).Summary);
                       });
    }

    public static IEnumerable<object[]> GetInfoOptions()
    {
        yield return new object[]
        {
            new BotInformation()
        };

        yield return new object[]
        {
            new BotInformation
            {
                AuthorInformation = new AuthorInformation
                {
                    AuthorName = "Soup",
                    AuthorAvatarUrl = @"https://www.google.com/",
                    AuthorUrl = @"https://www.bing.com/"
                },
                BotName = "Bot Name",
                BotInformationUrl = @"https://github.com/"
            }
        };
    }
}