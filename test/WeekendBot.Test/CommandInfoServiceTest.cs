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
using Discord;
using WeekendBot.Modules;
using Xunit;

namespace WeekendBot.Test;

public class CommandInfoServiceTest
{
    [Fact]
    public async Task GetCommandInfoSummaries_WithCommandInfos_ReturnsExpectedEmbed()
    {
        // Setup
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

        var service = new DiscordCommandInformationService();

        // Call
        Embed result = await service.GetCommandInfoSummaries(commandInfos);

        // Assert
        ImmutableArray<EmbedField> embedFields = result.Fields;
        Assert.Equal(commandInfos.Count(), embedFields.Length);

        int index = 0;
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
}