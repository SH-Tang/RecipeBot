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

using Discord.Commands;
using Discord.Common.Handlers;
using Discord.Common.InfoModule;
using Discord.Common.Options;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using NSubstitute;
using WeekendBot.TestUtils;
using Xunit;
using DiscordCommonInfoModule = Discord.Common.InfoModule.InfoModule;
using SummaryAttribute = Discord.Commands.SummaryAttribute;

namespace Discord.Common.Test.InfoModule;

public class InfoModuleTest
{
    [Fact]
    public void Constructor_WithArguments_ExpectedValues()
    {
        // Setup
        var commandService = Substitute.For<CommandService>();
        var discordCommandOptions = Substitute.For<IOptions<DiscordCommandOptions>>();

        var socketClient = Substitute.For<DiscordSocketClient>();
        var interactionService = new InteractionService(socketClient);

        var options = Substitute.For<IOptions<BotInformation>>();
        var infoService = new BotInformationService(options);

        // Call
        var module = new DiscordCommonInfoModule(commandService, interactionService, discordCommandOptions, infoService);

        // Assert
        Assert.IsAssignableFrom<ModuleBase<SocketCommandContext>>(module);
    }

    [Fact]
    public void GetHelpResponseAsync_Always_ReturnsExpectedAttributes()
    {
        // Call
        CommandAttribute? commandAttribute = ReflectionHelper.GetCustomAttribute<DiscordCommonInfoModule, CommandAttribute>(
            nameof(DiscordCommonInfoModule.GetHelpResponseAsync));
        SummaryAttribute? summaryAttribute = ReflectionHelper.GetCustomAttribute<DiscordCommonInfoModule, SummaryAttribute>(
            nameof(DiscordCommonInfoModule.GetHelpResponseAsync));

        // Assert
        Assert.NotNull(commandAttribute);
        Assert.Equal("help", commandAttribute!.Text.ToLower());

        Assert.NotNull(commandAttribute);
        const string expectedSummary = "Provides information about all the available commands.";
        Assert.Equal(expectedSummary, summaryAttribute!.Text);
    }
}