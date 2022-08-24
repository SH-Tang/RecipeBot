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

using Discord;
using Discord.Common;
using Discord.Interactions;
using NSubstitute;
using WeekendBot.Domain.Factories;
using WeekendBot.TestUtils;
using Xunit;

namespace WeekendBot.Discord.Test;

public class RecipeInteractionModuleTest
{
    [Fact]
    public void Module_is_interactive_module()
    {
        // Setup
        var loggingService = Substitute.For<ILoggingService>();
        var limitProvider = Substitute.For<IRecipeDomainEntityCharacterLimitProvider>();
        var responseService = new RecipeModalResponseService(limitProvider);

        // Call
        var module = new RecipeInteractionModule(loggingService, responseService);

        // Assert
        Assert.IsAssignableFrom<InteractionModuleBase<SocketInteractionContext>>(module);
    }

    [Fact]
    public void Format_recipe_command_has_expected_attributes()
    {
        // Call
        SlashCommandAttribute? commandAttribute = ReflectionHelper.GetCustomAttributeFromMethod<RecipeInteractionModule, SlashCommandAttribute>(
            nameof(RecipeInteractionModule.FormatRecipe), new[]
            {
                typeof(IAttachment)
            });

        // Assert
        Assert.NotNull(commandAttribute);
        Assert.Equal("recipe", commandAttribute!.Name);

        const string expectedDescription = "Tell us about your recipe";
        Assert.Equal(expectedDescription, commandAttribute.Description);
    }

    [Fact]
    public void Modal_response_responds_to_correct_modal_id()
    {
        // Call
        ModalInteractionAttribute? interactionAttribute = ReflectionHelper.GetCustomAttributeFromMethod<RecipeInteractionModule, ModalInteractionAttribute>(
            nameof(RecipeInteractionModule.OnModalResponse), new[]
            {
                typeof(RecipeModal)
            });

        // Assert
        Assert.NotNull(interactionAttribute);
        Assert.Equal(RecipeModal.ModalId, interactionAttribute!.CustomId);
    }
}