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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Discord;
using Discord.Common.Providers;
using Discord.Common.Services;
using Discord.Common.TestUtils;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using RecipeBot.Controllers;
using RecipeBot.Discord.Controllers;
using RecipeBot.Discord.Data;
using RecipeBot.Discord.Views;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Factories;
using RecipeBot.Domain.Models;
using RecipeBot.Domain.Repositories;
using RecipeBot.Domain.Repositories.Data;
using RecipeBot.Domain.TestUtils;
using RecipeBot.TestUtils;
using Xunit;

namespace RecipeBot.Test.Controllers;

public class RecipeControllerTest
{
    [Fact]
    public void Controller_is_recipe_controller()
    {
        // Setup
        var limitProvider = Substitute.For<IRecipeModelCharacterLimitProvider>();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        var repository = Substitute.For<IRecipeRepository>();
        var logger = Substitute.For<ILoggingService>();

        // Call
        var controller = new RecipeController(limitProvider, userDataProvider, repository, logger);

        // Assert
        controller.Should().BeAssignableTo<IRecipeController>();
    }

    [Fact]
    public async Task Recipe_with_valid_data_and_invalid_category_throws_exception_and_does_not_save()
    {
        // Setup
        const DiscordRecipeCategory category = (DiscordRecipeCategory)(-1);

        var fixture = new Fixture();
        var modal = fixture.Create<RecipeModal>();

        var user = Substitute.For<IUser>();
        user.Username.Returns("Recipe author");
        user.GetAvatarUrl().ReturnsForAnyArgs("https://AuthorImage.url");

        var repository = Substitute.For<IRecipeRepository>();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        var logger = Substitute.For<ILoggingService>();
        IRecipeModelCharacterLimitProvider limitProvider = CreateDiscordCharacterLimitProvider();

        var controller = new RecipeController(limitProvider, userDataProvider, repository, logger);

        // Call
        Func<Task> call = () => controller.SaveRecipeAsync(modal, user, category);

        // Assert
        await call.Should().ThrowAsync<InvalidEnumArgumentException>();
        await repository.DidNotReceiveWithAnyArgs().SaveRecipeAsync(Arg.Any<RecipeModel>());
    }

    [Fact]
    public async Task Recipe_with_valid_data_and_unsuccessfully_saved_logs_and_returns_result_with_error()
    {
        // Setup
        var fixture = new Fixture();
        var category = fixture.Create<DiscordRecipeCategory>();
        var modal = fixture.Create<RecipeModal>();

        var user = Substitute.For<IUser>();
        user.Username.Returns("Recipe author");
        user.GetAvatarUrl().ReturnsForAnyArgs("https://AuthorImage.url");

        var repository = Substitute.For<IRecipeRepository>();
        var exception = new RepositoryDataSaveException("Saving failed");
        repository.SaveRecipeAsync(Arg.Any<RecipeModel>()).ThrowsAsyncForAnyArgs(exception);

        IRecipeModelCharacterLimitProvider limitProvider = CreateDiscordCharacterLimitProvider();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<Embed> controllerResult = await controller.SaveRecipeAsync(modal, user, category);

        // Assert
        controllerResult.HasError.Should().BeTrue();
        controllerResult.ErrorMessage.Should().NotBeNullOrWhiteSpace();

        await logger.Received(1).LogErrorAsync(exception);
    }

    [Fact]
    public async Task Recipe_with_invalid_data_and_valid_category_logs_and_returns_result_with_error_and_does_not_save()
    {
        // Setup
        var fixture = new Fixture();
        var category = fixture.Create<DiscordRecipeCategory>();
        var modal = fixture.Create<RecipeModal>();

        var user = Substitute.For<IUser>();
        user.Username.Returns("Recipe author");
        user.GetAvatarUrl().ReturnsForAnyArgs("https://AuthorImage.url");

        var limitProvider = Substitute.For<IRecipeModelCharacterLimitProvider>();
        limitProvider.MaximumTitleLength.Returns(EmbedBuilder.MaxTitleLength);
        limitProvider.MaximumFieldNameLength.Returns(EmbedFieldBuilder.MaxFieldNameLength);
        limitProvider.MaximumFieldDataLength.Returns(EmbedFieldBuilder.MaxFieldValueLength);
        limitProvider.MaximumRecipeTagsLength.Returns(EmbedFooterBuilder.MaxFooterTextLength);

        var repository = Substitute.For<IRecipeRepository>();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<Embed> controllerResult = await controller.SaveRecipeAsync(modal, user, category);

        // Assert
        controllerResult.HasError.Should().BeTrue();
        controllerResult.ErrorMessage.Should().NotBeNullOrWhiteSpace();

        await repository.DidNotReceiveWithAnyArgs().SaveRecipeAsync(Arg.Any<RecipeModel>());
        await logger.Received(1).LogErrorAsync(Arg.Any<ModelCreateException>());
    }

    [Theory]
    [MemberData(nameof(GetDiscordRecipeCategoriesAndColor))]
    public async Task Recipe_with_valid_data_returns_result_with_embed(
        DiscordRecipeCategory category, Color expectedColor)
    {
        // Setup
        const string authorName = "Recipe author";
        const string authorImageUrl = "https://AuthorImage.url";
        var user = Substitute.For<IUser>();
        user.Username.Returns(authorName);
        user.GetAvatarUrl().ReturnsForAnyArgs(authorImageUrl);

        const string recipeTitle = "Recipe title";
        const string recipeIngredients = "My ingredients";
        const string recipeSteps = "My recipe steps";
        const string recipeNotes = "My notes";
        const string tags = "Tag1, TAG1, tag1, tag    1,      tag1, tag1      , tag2";
        var modal = new RecipeModal
        {
            RecipeTitle = recipeTitle,
            Ingredients = recipeIngredients,
            CookingSteps = recipeSteps,
            Notes = recipeNotes,
            Tags = tags
        };

        IRecipeModelCharacterLimitProvider limitProvider = CreateDiscordCharacterLimitProvider();
        var repository = Substitute.For<IRecipeRepository>();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<Embed> controllerResult = await controller.SaveRecipeAsync(modal, user, category);

        // Assert
        controllerResult.HasError.Should().BeFalse();

        Embed embedResult = controllerResult.Result!;
        AssertCommonEmbedResponseProperties(user, category, modal, expectedColor, embedResult);
        embedResult.Image.Should().BeNull();
    }

    [Fact]
    public async Task Recipe_with_valid_data_saves_correct_model()
    {
        // Setup
        var fixture = new Fixture();
        var category = fixture.Create<DiscordRecipeCategory>();

        const string authorName = "Recipe author";
        const string authorImageUrl = "https://AuthorImage.url";
        var user = Substitute.For<IUser>();
        user.Username.Returns(authorName);
        user.GetAvatarUrl().ReturnsForAnyArgs(authorImageUrl);

        const string recipeTitle = "Recipe title";
        const string recipeIngredients = "My ingredients";
        const string recipeSteps = "My recipe steps";
        const string recipeNotes = "My notes";
        const string tags = "Tag1, TAG1, tag1, tag    1,      tag1, tag1      , tag2";
        var modal = new RecipeModal
        {
            RecipeTitle = recipeTitle,
            Ingredients = recipeIngredients,
            CookingSteps = recipeSteps,
            Notes = recipeNotes,
            Tags = tags
        };

        var savedModels = new List<RecipeModel>();
        var repository = Substitute.For<IRecipeRepository>();
        repository.WhenForAnyArgs(r => r.SaveRecipeAsync(Arg.Any<RecipeModel>()))
                  .Do(c => savedModels.Add(c.Arg<RecipeModel>()));

        IRecipeModelCharacterLimitProvider limitProvider = CreateDiscordCharacterLimitProvider();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeController(limitProvider, userDataProvider, repository, logger);

        // Call
        await controller.SaveRecipeAsync(modal, user, category);

        // Assert
        RecipeModel savedModel = savedModels.Single();
        RecipeModelTestHelper.AssertFullRecipeProperties(user, category, modal, savedModel);
    }

    [Fact]
    public async Task Deleting_recipe_and_delete_successful_returns_result()
    {
        // Setup
        var fixture = new Fixture();
        var idToDelete = fixture.Create<long>();

        var deletedResult = fixture.Create<RecipeEntryRepositoryData>();
        var repository = Substitute.For<IRecipeRepository>();
        repository.DeleteRecipeAsync(idToDelete).Returns(deletedResult);

        UserData userData = UserDataTestFactory.CreateFullyConfigured();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(deletedResult.AuthorId).Returns(userData);

        IRecipeModelCharacterLimitProvider limitProvider = CreateDiscordCharacterLimitProvider();
        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<string> result = await controller.DeleteRecipeAsync(idToDelete);

        // Assert
        result.HasError.Should().BeFalse();

        result.Result.Should().Be($"Recipe titled '{deletedResult.Title}' with id '{deletedResult.EntityId}' and author '{userData.Username}' was successfully deleted.");
    }

    [Fact]
    public async Task Deleting_recipe_and_delete_unsuccessful_logs_and_returns_result_with_error()
    {
        // Setup
        var fixture = new Fixture();
        var exceptionMessage = fixture.Create<string>();

        var repository = Substitute.For<IRecipeRepository>();
        var exception = new RepositoryDataDeleteException(exceptionMessage);
        repository.DeleteRecipeAsync(Arg.Any<long>()).ThrowsAsyncForAnyArgs(exception);

        IRecipeModelCharacterLimitProvider limitProvider = CreateDiscordCharacterLimitProvider();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<string> result = await controller.DeleteRecipeAsync(fixture.Create<long>());

        // Assert
        result.HasError.Should().BeTrue();
        result.ErrorMessage.Should().Be(exceptionMessage);

        await logger.Received(1).LogErrorAsync(exception);
    }

    [Fact]
    public async Task Deleting_recipe_with_author_and_delete_successful_returns_result()
    {
        // Setup
        var fixture = new Fixture();
        var idToDelete = fixture.Create<long>();
        var authorId = fixture.Create<ulong>();

        var author = Substitute.For<IUser>();
        author.Id.Returns(authorId);

        var deletedResult = fixture.Create<RecipeEntryRepositoryData>();
        var repository = Substitute.For<IRecipeRepository>();
        repository.DeleteRecipeAsync(idToDelete, authorId).Returns(deletedResult);

        UserData userData = UserDataTestFactory.CreateFullyConfigured();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(authorId).Returns(userData);

        IRecipeModelCharacterLimitProvider limitProvider = CreateDiscordCharacterLimitProvider();
        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<string> result = await controller.DeleteRecipeAsync(idToDelete, author);

        // Assert
        result.HasError.Should().BeFalse();

        result.Result.Should().Be($"Recipe titled '{deletedResult.Title}' with id '{deletedResult.EntityId}' and author '{userData.Username}' was successfully deleted.");
    }

    [Fact]
    public async Task Deleting_recipe_with_author_and_delete_unsuccessful_logs_and_returns_result_with_error()
    {
        // Setup
        var fixture = new Fixture();
        var exceptionMessage = fixture.Create<string>();

        var repository = Substitute.For<IRecipeRepository>();
        var exception = new RepositoryDataDeleteException(exceptionMessage);
        repository.DeleteRecipeAsync(Arg.Any<long>(), Arg.Any<ulong>()).ThrowsAsyncForAnyArgs(exception);

        IRecipeModelCharacterLimitProvider limitProvider = CreateDiscordCharacterLimitProvider();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<string> result = await controller.DeleteRecipeAsync(fixture.Create<long>(), Substitute.For<IUser>());

        // Assert
        result.HasError.Should().BeTrue();
        result.ErrorMessage.Should().Be(exceptionMessage);

        await logger.Received(1).LogErrorAsync(exception);
    }

    [Fact]
    public async Task Retrieving_recipe_and_exception_thrown_when_retrieving_logs_and_returns_result_with_error()
    {
        // Setup
        var fixture = new Fixture();
        var exceptionMessage = fixture.Create<string>();

        var repository = Substitute.For<IRecipeRepository>();
        var exception = new RepositoryDataLoadException(exceptionMessage);
        repository.GetRecipeAsync(Arg.Any<long>()).ThrowsAsyncForAnyArgs(exception);

        IRecipeModelCharacterLimitProvider limitProvider = CreateDiscordCharacterLimitProvider();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<Embed> result = await controller.GetRecipeAsync(fixture.Create<long>());

        // Assert
        result.HasError.Should().BeTrue();
        result.ErrorMessage.Should().Be(exceptionMessage);

        await logger.Received(1).LogErrorAsync(exception);
    }

    [Fact]
    public async Task Retrieving_recipe_with_invalid_data_logs_and_returns_result_with_error()
    {
        // Setup
        var fixture = new Fixture();
        var idToRetrieve = fixture.Create<long>();

        var recipeData = fixture.Create<RecipeData>();

        var limitProvider = Substitute.For<IRecipeModelCharacterLimitProvider>();
        limitProvider.MaximumTitleLength.Returns(EmbedBuilder.MaxTitleLength);
        limitProvider.MaximumFieldDataLength.Returns(EmbedFieldBuilder.MaxFieldValueLength);
        limitProvider.MaximumRecipeTagsLength.Returns(EmbedFooterBuilder.MaxFooterTextLength);

        var repository = Substitute.For<IRecipeRepository>();
        repository.GetRecipeAsync(idToRetrieve).Returns(recipeData);

        var logger = Substitute.For<ILoggingService>();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        var controller = new RecipeController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<Embed> controllerResult = await controller.GetRecipeAsync(idToRetrieve);

        // Assert
        controllerResult.HasError.Should().BeTrue();
        controllerResult.ErrorMessage.Should().NotBeNullOrWhiteSpace();

        await logger.Received(1).LogErrorAsync(Arg.Any<ModelCreateException>());
    }

    [Fact]
    public async Task Retrieving_recipe_with_valid_data_and_tags_returns_result_with_embed()
    {
        // Setup
        var fixture = new Fixture();
        var idToRetrieve = fixture.Create<long>();
        var authorId = fixture.Create<ulong>();

        IEnumerable<RecipeFieldData> recipeFieldsData = fixture.CreateMany<RecipeFieldData>(3);

        var recipeTitle = fixture.Create<string>();
        const string tags = "Tag1, TAG1, tag1, tag    1,      tag1, tag1      , tag2";
        var recipeData = new RecipeData(authorId, recipeFieldsData, recipeTitle, fixture.Create<RecipeCategory>())
        {
            Tags = tags
        };

        IRecipeModelCharacterLimitProvider limitProvider = CreateDiscordCharacterLimitProvider();
        var repository = Substitute.For<IRecipeRepository>();
        repository.GetRecipeAsync(idToRetrieve).Returns(recipeData);

        UserData authorData = UserDataTestFactory.CreateFullyConfigured();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(authorId).Returns(authorData);

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<Embed> controllerResult = await controller.GetRecipeAsync(idToRetrieve);

        // Assert
        controllerResult.HasError.Should().BeFalse();

        Embed embedResult = controllerResult.Result!;
        AssertCommonEmbedResponseProperties(recipeData, authorData, embedResult);
    }

    [Fact]
    public async Task Retrieving_recipe_with_valid_data_and_without_tags_returns_result_with_embed()
    {
        // Setup
        var fixture = new Fixture();
        var idToRetrieve = fixture.Create<long>();
        var authorId = fixture.Create<ulong>();

        IEnumerable<RecipeFieldData> recipeFieldsData = fixture.CreateMany<RecipeFieldData>(3);

        var recipeTitle = fixture.Create<string>();
        var recipeData = new RecipeData(authorId, recipeFieldsData, recipeTitle, fixture.Create<RecipeCategory>());

        IRecipeModelCharacterLimitProvider limitProvider = CreateDiscordCharacterLimitProvider();
        var repository = Substitute.For<IRecipeRepository>();
        repository.GetRecipeAsync(idToRetrieve).Returns(recipeData);

        UserData authorData = UserDataTestFactory.CreateFullyConfigured();
        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(authorId).Returns(authorData);

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<Embed> controllerResult = await controller.GetRecipeAsync(idToRetrieve);

        // Assert
        controllerResult.HasError.Should().BeFalse();

        Embed embedResult = controllerResult.Result!;
        AssertCommonEmbedResponseProperties(recipeData, authorData, embedResult);
    }

    [Theory]
    [MemberData(nameof(GetRecipeCategoriesAndColor))]
    public async Task Retrieving_recipe_with_valid_data_returns_result_with_embed_in_correct_color(RecipeCategory category, Color expectedColor)
    {
        // Setup
        var fixture = new Fixture();
        var authorId = fixture.Create<ulong>();

        IEnumerable<RecipeFieldData> recipeFieldsData = fixture.CreateMany<RecipeFieldData>(3);

        var recipeTitle = fixture.Create<string>();
        var recipeData = new RecipeData(authorId, recipeFieldsData, recipeTitle, category);

        IRecipeModelCharacterLimitProvider limitProvider = CreateDiscordCharacterLimitProvider();
        var repository = Substitute.For<IRecipeRepository>();
        repository.GetRecipeAsync(Arg.Any<long>()).ReturnsForAnyArgs(recipeData);

        var userDataProvider = Substitute.For<IUserDataProvider>();
        userDataProvider.GetUserDataAsync(authorId).Returns(UserDataTestFactory.CreateFullyConfigured());

        var logger = Substitute.For<ILoggingService>();
        var controller = new RecipeController(limitProvider, userDataProvider, repository, logger);

        // Call
        ControllerResult<Embed> controllerResult = await controller.GetRecipeAsync(fixture.Create<long>());

        // Assert
        controllerResult.HasError.Should().BeFalse();

        Embed embedResult = controllerResult.Result!;
        embedResult.Color.Should().Be(expectedColor);
    }

    public static IEnumerable<object[]> GetDiscordRecipeCategoriesAndColor()
    {
        yield return new object[]
        {
            DiscordRecipeCategory.Meat,
            new Color(250, 85, 87)
        };

        yield return new object[]
        {
            DiscordRecipeCategory.Fish,
            new Color(86, 153, 220)
        };

        yield return new object[]
        {
            DiscordRecipeCategory.Vegetarian,
            new Color(206, 221, 85)
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Vegan,
            new Color(6, 167, 125)
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Drinks,
            new Color(175, 234, 224)
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Pastry,
            new Color(206, 132, 173)
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Dessert,
            new Color(176, 69, 162)
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Snack,
            new Color(249, 162, 114)
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Other,
            new Color(165, 161, 164)
        };
    }

    public static IEnumerable<object[]> GetRecipeCategoriesAndColor()
    {
        yield return new object[]
        {
            RecipeCategory.Meat,
            new Color(250, 85, 87)
        };

        yield return new object[]
        {
            RecipeCategory.Fish,
            new Color(86, 153, 220)
        };

        yield return new object[]
        {
            RecipeCategory.Vegetarian,
            new Color(206, 221, 85)
        };
        yield return new object[]
        {
            RecipeCategory.Vegan,
            new Color(6, 167, 125)
        };
        yield return new object[]
        {
            RecipeCategory.Drinks,
            new Color(175, 234, 224)
        };
        yield return new object[]
        {
            RecipeCategory.Pastry,
            new Color(206, 132, 173)
        };
        yield return new object[]
        {
            RecipeCategory.Dessert,
            new Color(176, 69, 162)
        };
        yield return new object[]
        {
            RecipeCategory.Snack,
            new Color(249, 162, 114)
        };
        yield return new object[]
        {
            RecipeCategory.Other,
            new Color(165, 161, 164)
        };
    }

    private static IRecipeModelCharacterLimitProvider CreateDiscordCharacterLimitProvider()
    {
        var limitProvider = Substitute.For<IRecipeModelCharacterLimitProvider>();
        limitProvider.MaximumTitleLength.Returns(EmbedBuilder.MaxTitleLength);
        limitProvider.MaximumRecipeLength.Returns(EmbedBuilder.MaxEmbedLength);
        limitProvider.MaximumFieldNameLength.Returns(EmbedFieldBuilder.MaxFieldNameLength);
        limitProvider.MaximumFieldDataLength.Returns(EmbedFieldBuilder.MaxFieldValueLength);
        limitProvider.MaximumRecipeTagsLength.Returns(EmbedFooterBuilder.MaxFooterTextLength);
        return limitProvider;
    }

    private static void AssertCommonEmbedResponseProperties(
        IUser user, DiscordRecipeCategory category, RecipeModal modal, Color color, IEmbed actualResponse)
    {
        actualResponse.Title.Should().Be(modal.RecipeTitle);
        actualResponse.Color.Should().Be(color);

        EmbedAuthor? actualResponseAuthor = actualResponse.Author;
        actualResponseAuthor.Should().NotBeNull();
        AssertAuthor(user.Username, user.GetAvatarUrl(), actualResponseAuthor!.Value);

        actualResponse.Fields.Should().SatisfyRespectively(
            firstField => AssertField("Ingredients", modal.Ingredients, firstField),
            secondField => AssertField("Cooking steps", modal.CookingSteps, secondField),
            thirdField => AssertField("Additional notes", modal.Notes, thirdField));

        EmbedFooter? actualResponseFooter = actualResponse.Footer;
        actualResponseFooter.Should().NotBeNull();
        AssertTags(category, modal.Tags, actualResponseFooter!.Value);
    }

    private static void AssertCommonEmbedResponseProperties(RecipeData recipeData, UserData authorData, IEmbed actualResponse)
    {
        actualResponse.Title.Should().Be(recipeData.RecipeTitle);

        EmbedAuthor? actualResponseAuthor = actualResponse.Author;
        actualResponseAuthor.Should().NotBeNull();

        AssertAuthor(authorData.Username, authorData.UserImageUrl, actualResponseAuthor!.Value);

        actualResponse.Fields.Should().BeEquivalentTo(recipeData.RecipeFields,
                                                      options => options.ExcludingMissingMembers()
                                                                        .WithMapping<EmbedField, RecipeFieldData>(s => s.Name, e => e.FieldName)
                                                                        .WithMapping<EmbedField, RecipeFieldData>(s => s.Value, e => e.FieldData));

        EmbedFooter? actualResponseFooter = actualResponse.Footer;
        if (!string.IsNullOrWhiteSpace(recipeData.Tags))
        {
            actualResponseFooter.Should().NotBeNull();
            AssertTags(recipeData.Category, recipeData.Tags, actualResponseFooter!.Value);
        }
        else
        {
            actualResponseFooter.Should().BeNull();
        }
    }

    private static void AssertAuthor(string expectedAuthorName, string? expectedAuthorImageUrl, EmbedAuthor actualAuthor)
    {
        actualAuthor.Name.Should().Be(expectedAuthorName);
        actualAuthor.IconUrl.Should().Be(expectedAuthorImageUrl);
    }

    private static void AssertField(string expectedName, string? expectedValue, EmbedField actualField)
    {
        actualField.Name.Should().Be(expectedName);
        actualField.Value.Should().Be(expectedValue);
        actualField.Inline.Should().BeFalse();
    }

    private static void AssertTags(DiscordRecipeCategory category, string? tags, EmbedFooter actualFooter)
    {
        var expectedTags = new List<string>
        {
            DiscordRecipeCategoryTestHelper.CategoryStringMapping[category]
        };

        if (!string.IsNullOrWhiteSpace(tags))
        {
            expectedTags.AddRange(TagTestHelper.GetParsedTags(tags));
        }

        string expectedFooterText = string.Join(", ", expectedTags);
        actualFooter.Text.Should().Be(expectedFooterText);
    }

    private static void AssertTags(RecipeCategory category, string? tags, EmbedFooter actualFooter)
    {
        var expectedTags = new List<string>
        {
            RecipeCategoryTestHelper.CategoryStringMapping[category]
        };

        if (!string.IsNullOrWhiteSpace(tags))
        {
            expectedTags.AddRange(TagTestHelper.GetParsedTags(tags));
        }

        string expectedFooterText = string.Join(", ", expectedTags);
        actualFooter.Text.Should().Be(expectedFooterText);
    }
}