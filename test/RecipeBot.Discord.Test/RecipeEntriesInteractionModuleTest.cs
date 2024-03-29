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

using Discord.Common.Services;
using Discord.Interactions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using RecipeBot.Discord.Data;
using RecipeBot.TestUtils;
using Xunit;

namespace RecipeBot.Discord.Test;

public class RecipeEntriesInteractionModuleTest
{
    [Fact]
    public void Module_is_interactive_module()
    {
        // Setup
        var scopeFactory = Substitute.For<IServiceScopeFactory>();
        var loggingService = Substitute.For<ILoggingService>();

        // Call
        var module = new RecipeEntriesInteractionModule(scopeFactory, loggingService);

        // Assert
        module.Should().BeAssignableTo<DiscordInteractionModuleBase>();
        module.Should().BeAssignableTo<InteractionModuleBase<SocketInteractionContext>>();
    }

    [Fact]
    public void List_recipes_command_has_expected_attributes()
    {
        // Call
        SlashCommandAttribute? commandAttribute = ReflectionHelper.GetCustomAttributeFromMethod<RecipeEntriesInteractionModule, SlashCommandAttribute>(
            nameof(RecipeEntriesInteractionModule.GetAllRecipes));

        // Assert
        const string expectedName = "recipe-list";
        const string expectedDescription = "Lists all the saved user recipes";

        commandAttribute.Should().NotBeNull();
        commandAttribute!.Name.Should().Be(expectedName);
        commandAttribute.Description.Should().Be(expectedDescription);
    }

    [Fact]
    public void List_recipes_by_category_command_has_expected_attributes()
    {
        // Call
        SlashCommandAttribute? commandAttribute = ReflectionHelper.GetCustomAttributeFromMethod<RecipeEntriesInteractionModule, SlashCommandAttribute>(
            nameof(RecipeEntriesInteractionModule.GetAllRecipeByCategory), new[]
            {
                typeof(DiscordRecipeCategory)
            });

        // Assert
        const string expectedName = "recipe-list-by-category";
        const string expectedDescription = "Lists all the saved user recipes filtered by category";

        commandAttribute.Should().NotBeNull();
        commandAttribute!.Name.Should().Be(expectedName);
        commandAttribute.Description.Should().Be(expectedDescription);
    }

    [Fact]
    public void List_recipes_by_tag_id_command_has_expected_attributes()
    {
        // Call
        SlashCommandAttribute? commandAttribute = ReflectionHelper.GetCustomAttributeFromMethod<RecipeEntriesInteractionModule, SlashCommandAttribute>(
            nameof(RecipeEntriesInteractionModule.GetAllRecipeByTagId), new[]
            {
                typeof(long)
            });

        // Assert
        const string expectedName = "recipe-list-by-tag-id";
        const string expectedDescription = "Lists all the saved user recipes filtered by tag id";

        commandAttribute.Should().NotBeNull();
        commandAttribute!.Name.Should().Be(expectedName);
        commandAttribute.Description.Should().Be(expectedDescription);
    }

    [Fact]
    public void List_recipes_by_tag_command_has_expected_attributes()
    {
        // Call
        SlashCommandAttribute? commandAttribute = ReflectionHelper.GetCustomAttributeFromMethod<RecipeEntriesInteractionModule, SlashCommandAttribute>(
            nameof(RecipeEntriesInteractionModule.GetAllRecipeByTag), new[]
            {
                typeof(string)
            });

        // Assert
        const string expectedName = "recipe-list-by-tag";
        const string expectedDescription = "Lists all the saved user recipes filtered by tag";

        commandAttribute.Should().NotBeNull();
        commandAttribute!.Name.Should().Be(expectedName);
        commandAttribute.Description.Should().Be(expectedDescription);
    }

    [Fact]
    public void List_recipes_by_user_command_has_expected_attributes()
    {
        // Call
        SlashCommandAttribute? commandAttribute = ReflectionHelper.GetCustomAttributeFromMethod<RecipeEntriesInteractionModule, SlashCommandAttribute>(
            nameof(RecipeEntriesInteractionModule.GetAllRecipeByUser));

        // Assert
        const string expectedName = "myrecipes-list";
        const string expectedDescription = "Lists all your saved user recipes";

        commandAttribute.Should().NotBeNull();
        commandAttribute!.Name.Should().Be(expectedName);
        commandAttribute.Description.Should().Be(expectedDescription);
    }
}