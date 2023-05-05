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

using Discord;
using Discord.Common;
using Discord.Interactions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using RecipeBot.Discord.Data;
using RecipeBot.Discord.Views;
using RecipeBot.TestUtils;
using Xunit;

namespace RecipeBot.Discord.Test;

public class RecipeInteractionModuleTest
{
    [Fact]
    public void Module_is_interactive_module()
    {
        // Setup
        var scopeFactory = Substitute.For<IServiceScopeFactory>();
        var loggingService = Substitute.For<ILoggingService>();

        // Call
        var module = new RecipeInteractionModule(scopeFactory, loggingService);

        // Assert
        module.Should().BeAssignableTo<InteractionModuleBase<SocketInteractionContext>>();
    }

    [Fact]
    public void Save_recipe_command_has_expected_attributes()
    {
        // Call
        SlashCommandAttribute? commandAttribute = ReflectionHelper.GetCustomAttributeFromMethod<RecipeInteractionModule, SlashCommandAttribute>(
            nameof(RecipeInteractionModule.SaveRecipe), new[]
            {
                typeof(DiscordRecipeCategory),
                typeof(IAttachment)
            });

        // Assert
        const string expectedName = "recipe";
        const string expectedDescription = "Formats and stores an user recipe";

        commandAttribute.Should().NotBeNull();
        commandAttribute!.Name.Should().Be(expectedName);
        commandAttribute.Description.Should().Be(expectedDescription);
    }

    [Fact]
    public void Delete_recipe_command_has_expected_attributes()
    {
        // Call
        SlashCommandAttribute? commandAttribute = ReflectionHelper.GetCustomAttributeFromMethod<RecipeInteractionModule, SlashCommandAttribute>(
            nameof(RecipeInteractionModule.DeleteRecipe), new[]
            {
                typeof(long)
            });

        DefaultMemberPermissionsAttribute? permissionAttribute = ReflectionHelper.GetCustomAttributeFromMethod<RecipeInteractionModule, DefaultMemberPermissionsAttribute>(
            nameof(RecipeInteractionModule.DeleteRecipe), new[]
            {
                typeof(long)
            });

        // Assert
        const string expectedName = "recipe-delete";
        const string expectedDescription = "Deletes a recipe based on the id";

        commandAttribute.Should().NotBeNull();
        commandAttribute!.Name.Should().Be(expectedName);
        commandAttribute.Description.Should().Be(expectedDescription);

        permissionAttribute.Should().NotBeNull();
        permissionAttribute!.Permissions.Should().Be(GuildPermission.Administrator | GuildPermission.ModerateMembers);
    }

    [Fact]
    public void Get_recipe_command_has_expected_attributes()
    {
        // Call
        SlashCommandAttribute? commandAttribute = ReflectionHelper.GetCustomAttributeFromMethod<RecipeInteractionModule, SlashCommandAttribute>(
            nameof(RecipeInteractionModule.GetRecipe), new[]
            {
                typeof(long)
            });

        // Assert
        const string expectedName = "recipe-get";
        const string expectedDescription = "Gets a recipe based on the id";

        commandAttribute.Should().NotBeNull();
        commandAttribute!.Name.Should().Be(expectedName);
        commandAttribute.Description.Should().Be(expectedDescription);
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
        interactionAttribute.Should().NotBeNull();
        interactionAttribute!.CustomId.Should().Be(RecipeModal.ModalId);
    }
}