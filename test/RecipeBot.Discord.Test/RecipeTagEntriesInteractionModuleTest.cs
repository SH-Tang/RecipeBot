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
using RecipeBot.TestUtils;
using Xunit;

namespace RecipeBot.Discord.Test;

public class RecipeTagEntriesInteractionModuleTest
{
    [Fact]
    public void Module_is_interactive_module()
    {
        // Setup
        var scopeFactory = Substitute.For<IServiceScopeFactory>();
        var loggingService = Substitute.For<ILoggingService>();

        // Call
        var module = new RecipeTagEntriesInteractionModule(scopeFactory, loggingService);

        // Assert
        module.Should().BeAssignableTo<InteractionModuleBase<SocketInteractionContext>>();
    }

    [Fact]
    public void List_tags_command_has_expected_attributes()
    {
        // Call
        SlashCommandAttribute? commandAttribute = ReflectionHelper.GetCustomAttributeFromMethod<RecipeTagEntriesInteractionModule, SlashCommandAttribute>(
            nameof(RecipeTagEntriesInteractionModule.ListTags));

        // Assert
        const string expectedName = "tag-list";
        const string expectedDescription = "Lists all the saved tags";

        commandAttribute.Should().NotBeNull();
        commandAttribute!.Name.Should().Be(expectedName);
        commandAttribute.Description.Should().Be(expectedDescription);
    }

    [Fact]
    public void Delete_tag_command_has_expected_attributes()
    {
        // Call
        SlashCommandAttribute? commandAttribute = ReflectionHelper.GetCustomAttributeFromMethod<RecipeTagEntriesInteractionModule, SlashCommandAttribute>(
            nameof(RecipeTagEntriesInteractionModule.DeleteTag), new []
            {
                typeof(long)
            });

        DefaultMemberPermissionsAttribute? permissionAttribute = ReflectionHelper.GetCustomAttributeFromMethod<RecipeTagEntriesInteractionModule, DefaultMemberPermissionsAttribute>(
            nameof(RecipeTagEntriesInteractionModule.DeleteTag), new[]
            {
                typeof(long)
            });


        // Assert
        const string expectedName = "tag-delete";
        const string expectedDescription = "Deletes a tag based on the id";

        commandAttribute.Should().NotBeNull();
        commandAttribute!.Name.Should().Be(expectedName);
        commandAttribute.Description.Should().Be(expectedDescription);

        permissionAttribute.Should().NotBeNull();
        permissionAttribute!.Permissions.Should().Be(GuildPermission.Administrator | GuildPermission.ModerateMembers);
    }
}