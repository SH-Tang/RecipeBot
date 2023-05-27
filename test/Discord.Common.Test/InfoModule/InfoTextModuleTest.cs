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

using Discord.Commands;
using Discord.Common.InfoModule;
using Discord.Common.Services;
using FluentAssertions;
using NSubstitute;
using RecipeBot.TestUtils;
using Xunit;

namespace Discord.Common.Test.InfoModule;

public class InfoTextModuleTest
{
    [Fact]
    public void Module_is_text_based_module()
    {
        // Setup
        var controller = Substitute.For<IDiscordBotInformationController>();
        var logging = Substitute.For<ILoggingService>();

        // Call
        var module = new InfoTextModule(controller, logging);

        // Assert
        module.Should().BeAssignableTo<ModuleBase<SocketCommandContext>>();
    }

    [Fact]
    public void Help_command_has_expected_summary()
    {
        // Call
        CommandAttribute? commandAttribute = ReflectionHelper.GetCustomAttributeFromMethod<InfoTextModule, CommandAttribute>(
            nameof(InfoTextModule.GetHelpResponseAsync));
        SummaryAttribute? summaryAttribute = ReflectionHelper.GetCustomAttributeFromMethod<InfoTextModule, SummaryAttribute>(
            nameof(InfoTextModule.GetHelpResponseAsync));

        // Assert
        commandAttribute.Should().NotBeNull();
        commandAttribute!.Text.Should().Be("help");

        const string expectedSummary = "Provides information about all the available commands.";
        summaryAttribute.Should().NotBeNull();
        summaryAttribute!.Text.Should().Be(expectedSummary);
    }
}