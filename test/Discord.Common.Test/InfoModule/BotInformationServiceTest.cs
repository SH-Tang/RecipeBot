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
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Discord.Common.InfoModule;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Discord.Common.Test.InfoModule;

public class BotInformationServiceTest
{
    [Theory]
    [MemberData(nameof(GetInfoOptions))]
    public async Task GetCommandInfoSummaries_WithOptions_ReturnsEmbedWithExpectedMetaData(
        BotInformationOptions botInformationOptions)
    {
        // Setup
        var options = Substitute.For<IOptions<BotInformationOptions>>();
        options.Value.Returns(botInformationOptions);

        var service = new BotInformationService(options);

        // Call
        Embed result = await service.GetCommandInfoSummaries(Enumerable.Empty<DiscordCommandInformation>());

        // Assert
        Assert.Equal(Color.Blue, result.Color);

        string expectedTitle = botInformationOptions.BotInformationUrl == null
                                   ? "Available commands"
                                   : $"Available commands for {botInformationOptions.BotName}";
        Assert.Equal(expectedTitle, result.Title);
        Assert.Equal(botInformationOptions.BotInformationUrl, result.Url);

        AuthorInformation? authorInformation = botInformationOptions.AuthorInformation;
        Assert.NotNull(result.Author);
        EmbedAuthor embedAuthor = result.Author!.Value;
        Assert.Equal(authorInformation?.AuthorName, embedAuthor.Name);
        Assert.Equal(authorInformation?.AuthorUrl, embedAuthor.Url);
        Assert.Equal(authorInformation?.AuthorAvatarUrl, embedAuthor.IconUrl);
    }

    [Fact]
    public async Task GetCommandInfoSummaries_WithCommandInfos_ReturnsExpectedEmbedFields()
    {
        // Setup
        var options = Substitute.For<IOptions<BotInformationOptions>>();
        options.Value.Returns(new BotInformationOptions());

        var service = new BotInformationService(options);

        var infoWithoutSummary = new DiscordCommandInformation("Command1");
        var infoWithSummary = new DiscordCommandInformation("Command2")
        {
            Summary = "Summary"
        };
        IEnumerable<DiscordCommandInformation> commandInfos = new[]
        {
            infoWithoutSummary,
            infoWithSummary
        };

        // Call
        Embed result = await service.GetCommandInfoSummaries(commandInfos);

        // Assert
        ImmutableArray<EmbedField> embedFields = result.Fields;
        Assert.Equal(commandInfos.Count(), embedFields.Length);

        var index = 0;
        foreach (EmbedField embedField in embedFields)
        {
            AssertEmbedField(embedField, commandInfos.ElementAt(index++));
        }
    }

    private static void AssertEmbedField(EmbedField field, DiscordCommandInformation info)
    {
        Assert.Equal(field.Name, info.Name);

        string expectedSummary = info.Summary ?? $"No description available.{Environment.NewLine}";
        Assert.Equal(expectedSummary, field.Value);
    }

    private static IEnumerable<object[]> GetInfoOptions()
    {
        yield return new object[]
        {
            new BotInformationOptions()
        };

        yield return new object[]
        {
            new BotInformationOptions
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