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
using Discord.Interactions;
using FluentAssertions;
using NSubstitute;
using RecipeBot.TestUtils;
using Xunit;

namespace Discord.Common.Test.InfoModule;

public class InfoInteractionModuleTest
{
    [Fact]
    public void Module_is_interactive_module()
    {
        // Setup
        var controller = Substitute.For<IDiscordBotInformationController>();
        var logger = Substitute.For<ILoggingService>();

        // Call
        var module = new InfoInteractionModule(controller, logger);

        // Assert
        module.Should().BeAssignableTo<InteractionModuleBase<SocketInteractionContext>>();
    }

    [Fact]
    public void Help_command_has_expected_description()
    {
        // Call
        SlashCommandAttribute? commandAttribute = ReflectionHelper.GetCustomAttributeFromMethod<InfoInteractionModule, SlashCommandAttribute>(
            nameof(InfoInteractionModule.GetHelpResponseAsync));

        // Assert
        commandAttribute.Should().NotBeNull();
        commandAttribute!.Name.Should().Be("help");

        const string expectedDescription = "Provides information about all the available commands.";
        commandAttribute.Description.Should().Be(expectedDescription);
    }
}