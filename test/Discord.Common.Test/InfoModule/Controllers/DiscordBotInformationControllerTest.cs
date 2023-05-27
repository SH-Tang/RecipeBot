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

using Discord.Common.InfoModule;
using Discord.Common.InfoModule.Controllers;
using Discord.Common.InfoModule.Data;
using Discord.Interactions;
using Discord.WebSocket;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Discord.Common.Test.InfoModule.Controllers;

public class DiscordBotInformationControllerTest
{
    [Fact]
    public void Controller_is_bot_information_controller()
    {
        // Setup
        var socketClient = Substitute.For<DiscordSocketClient>();
        var interactionService = new InteractionService(socketClient);

        var botInformation = Substitute.For<IOptions<BotInformation>>();

        // Call
        var controller = new DiscordBotInformationController(interactionService, botInformation);

        // Assert
        controller.Should().BeAssignableTo<IDiscordBotInformationController>();
    }
}