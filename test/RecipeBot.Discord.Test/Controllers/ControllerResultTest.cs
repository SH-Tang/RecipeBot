﻿// Copyright (C) 2022 Dennis Tang. All rights reserved.
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

using AutoFixture;
using FluentAssertions;
using RecipeBot.Discord.Controllers;
using Xunit;

namespace RecipeBot.Discord.Test.Controllers;

public class ControllerResultTest
{
    [Fact]
    public void Given_result_with_result_sets_expected_properties()
    {
        // Setup
        var result = new object();

        // Call
        ControllerResult<object> controllerResult = ControllerResult<object>.CreateControllerResultWithValidResult(result);

        // Assert
        controllerResult.Result.Should().BeSameAs(result);
        controllerResult.HasError.Should().BeFalse();
        controllerResult.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void Given_result_with_error_message_sets_expected_properties()
    {
        // Setup
        var fixture = new Fixture();
        var errorMessage = fixture.Create<string>();

        // Call
        ControllerResult<object> controllerResult = ControllerResult<object>.CreateControllerResultWithError(errorMessage);

        // Assert
        controllerResult.Result.Should().BeNull();
        controllerResult.HasError.Should().BeTrue();
        controllerResult.ErrorMessage.Should().Be(errorMessage);
    }
}