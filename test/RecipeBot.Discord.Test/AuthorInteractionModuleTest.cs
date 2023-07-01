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
using Discord.Common.Services;
using Discord.Interactions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using RecipeBot.TestUtils;
using Xunit;

namespace RecipeBot.Discord.Test;

public class AuthorInteractionModuleTest
{
    [Fact]
    public void Module_is_interactive_module()
    {
        // Setup
        var scopeFactory = Substitute.For<IServiceScopeFactory>();
        var loggingService = Substitute.For<ILoggingService>();

        // Call
        var module = new AuthorInteractionModule(scopeFactory, loggingService);

        // Assert
        module.Should().BeAssignableTo<InteractionModuleBase<SocketInteractionContext>>();
    }

    [Fact]
    public void Delete_author_command_has_expected_attributes()
    {
        // Call
        SlashCommandAttribute? commandAttribute = ReflectionHelper.GetCustomAttributeFromMethod<AuthorInteractionModule, SlashCommandAttribute>(
            nameof(AuthorInteractionModule.DeleteAuthor));

        // Assert
        const string expectedName = "myuserdata-delete-all";
        const string expectedDescription = "Deletes all user data";

        commandAttribute.Should().NotBeNull();
        commandAttribute!.Name.Should().Be(expectedName);
        commandAttribute.Description.Should().Be(expectedDescription);
    }

    [Fact]
    public void List_authors_command_has_expected_attributes()
    {
        // Call
        SlashCommandAttribute? commandAttribute = ReflectionHelper.GetCustomAttributeFromMethod<AuthorInteractionModule, SlashCommandAttribute>(
            nameof(AuthorInteractionModule.ListAuthors));

        DefaultMemberPermissionsAttribute? permissionAttribute = ReflectionHelper.GetCustomAttributeFromMethod<AuthorInteractionModule, DefaultMemberPermissionsAttribute>(
            nameof(AuthorInteractionModule.ListAuthors));

        // Assert
        const string expectedName = "author-list";
        const string expectedDescription = "Lists all stored authors in the database";

        commandAttribute.Should().NotBeNull();
        commandAttribute!.Name.Should().Be(expectedName);
        commandAttribute.Description.Should().Be(expectedDescription);

        permissionAttribute.Should().NotBeNull();
        permissionAttribute!.Permissions.Should().Be(GuildPermission.Administrator | GuildPermission.ModerateMembers);
    }
}