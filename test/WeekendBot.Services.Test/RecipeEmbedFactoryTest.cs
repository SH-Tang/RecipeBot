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

using System;
using System.Collections.Generic;
using Discord;
using NSubstitute;
using WeekendBot.Domain;
using WeekendBot.Domain.Data;
using Xunit;

namespace WeekendBot.Services.Test;

public class RecipeEmbedFactoryTest
{
    [Fact]
    public void Basic_recipe_should_return_embed_without_image_and_notes()
    {
        // Setup
        const string authorName = "Recipe author";
        const string authorImageUrl = "https://AuthorImage.url";
        var authorData = new AuthorData(authorName, authorImageUrl);

        const string recipeTitle = "Recipe title";
        const string recipeIngredients = "My ingredients";
        const string recipeSteps = "My recipe steps";

        RecipeData recipeData = new RecipeDataBuilder(authorData, recipeTitle, recipeIngredients, recipeSteps)
            .Build();

        // Call
        Embed embed = RecipeEmbedFactory.Create(recipeData);

        // Assert
        EmbedAuthor? embedAuthor = embed.Author;
        Assert.NotNull(embedAuthor);
        AssertAuthor(authorName, authorImageUrl, embedAuthor!.Value);

        Assert.Equal(recipeTitle, embed.Title);
        Assert.Null(embed.Image);

        Assert.Equal(2, embed.Fields.Length);
        AssertField("Ingredients", recipeIngredients, embed.Fields[0]);
        AssertField("Cooking steps", recipeSteps, embed.Fields[1]);
    }

    [Fact]
    public void Recipe_with_notes_and_image_should_return_embed_with_notes_and_image()
    {
        // Setup
        const string authorName = "Recipe author";
        const string authorImageUrl = "https://AuthorImage.url";
        var authorData = new AuthorData(authorName, authorImageUrl);

        const string recipeTitle = "Recipe title";
        const string recipeIngredients = "My ingredients";
        const string recipeSteps = "My recipe steps";
        const string recipeNotes = "My notes";

        const string recipeImageUrl = "https://RecipeImage.url";
        var attachment = Substitute.For<IAttachment>();
        attachment.ContentType.Returns("image/");
        attachment.Url.Returns(recipeImageUrl);

        RecipeData recipeData = new RecipeDataBuilder(authorData, recipeTitle, recipeIngredients, recipeSteps)
                                .AddImage(attachment)
                                .AddNotes(recipeNotes)
                                .Build();

        // Call
        Embed embed = RecipeEmbedFactory.Create(recipeData);

        // Assert
        EmbedAuthor? embedAuthor = embed.Author;
        Assert.NotNull(embedAuthor);
        AssertAuthor(authorName, authorImageUrl, embedAuthor!.Value);

        Assert.Equal(recipeTitle, embed.Title);

        EmbedImage? embedImage = embed.Image;
        Assert.NotNull(embedImage);
        Assert.Equal(recipeImageUrl, embedImage!.Value.Url);

        Assert.Equal(3, embed.Fields.Length);
        AssertField("Ingredients", recipeIngredients, embed.Fields[0]);
        AssertField("Cooking steps", recipeSteps, embed.Fields[1]);
        AssertField("Additional notes", recipeNotes, embed.Fields[2]);
    }

    [Fact]
    public void Recipe_with_image_should_return_embed_with_image()
    {
        // Setup
        const string authorName = "Recipe author";
        const string authorImageUrl = "https://AuthorImage.url";
        var authorData = new AuthorData(authorName, authorImageUrl);

        const string recipeTitle = "Recipe title";
        const string recipeIngredients = "My ingredients";
        const string recipeSteps = "My recipe steps";

        const string recipeImageUrl = "https://RecipeImage.url";
        var attachment = Substitute.For<IAttachment>();
        attachment.ContentType.Returns("image/");
        attachment.Url.Returns(recipeImageUrl);

        RecipeData recipeData = new RecipeDataBuilder(authorData, recipeTitle, recipeIngredients, recipeSteps)
                                .AddImage(attachment)
                                .Build();

        // Call
        Embed embed = RecipeEmbedFactory.Create(recipeData);

        // Assert
        EmbedAuthor? embedAuthor = embed.Author;
        Assert.NotNull(embedAuthor);
        AssertAuthor(authorName, authorImageUrl, embedAuthor!.Value);

        Assert.Equal(recipeTitle, embed.Title);

        EmbedImage? embedImage = embed.Image;
        Assert.NotNull(embedImage);
        Assert.Equal(recipeImageUrl, embedImage!.Value.Url);

        Assert.Equal(2, embed.Fields.Length);
        AssertField("Ingredients", recipeIngredients, embed.Fields[0]);
        AssertField("Cooking steps", recipeSteps, embed.Fields[1]);
    }

    [Fact]
    public void Recipe_with_notes_should_return_embed_with_notes()
    {
        // Setup
        const string authorName = "Recipe author";
        const string authorImageUrl = "https://AuthorImage.url";
        var authorData = new AuthorData(authorName, authorImageUrl);

        const string recipeTitle = "Recipe title";
        const string recipeIngredients = "My ingredients";
        const string recipeSteps = "My recipe steps";
        const string recipeNotes = "My notes";

        RecipeData recipeData = new RecipeDataBuilder(authorData, recipeTitle, recipeIngredients, recipeSteps)
                                .AddNotes(recipeNotes)
                                .Build();

        // Call
        Embed embed = RecipeEmbedFactory.Create(recipeData);

        // Assert
        EmbedAuthor? embedAuthor = embed.Author;
        Assert.NotNull(embedAuthor);
        AssertAuthor(authorName, authorImageUrl, embedAuthor!.Value);

        Assert.Equal(recipeTitle, embed.Title);
        Assert.Null(embed.Image);

        Assert.Equal(3, embed.Fields.Length);
        AssertField("Ingredients", recipeIngredients, embed.Fields[0]);
        AssertField("Cooking steps", recipeSteps, embed.Fields[1]);
        AssertField("Additional notes", recipeNotes, embed.Fields[2]);
    }

    [Theory]
    [MemberData(nameof(GetInvalidFieldCharacterLengths))]
    public void Recipe_with_properties_with_invalid_character_count_throws_exception(
        string recipeIngredients, string recipeSteps, string recipeNotes)
    {
        // Setup
        const string authorName = "Recipe author";
        const string authorImageUrl = "https://AuthorImage.url";
        var authorData = new AuthorData(authorName, authorImageUrl);

        const string recipeTitle = "Recipe title";
        RecipeData recipeData = new RecipeDataBuilder(authorData, recipeTitle, recipeIngredients, recipeSteps)
                                .AddNotes(recipeNotes)
                                .Build();

        // Call
        Func<Embed> call = () => RecipeEmbedFactory.Create(recipeData);

        // Assert
        var exception = Assert.Throws<ModalResponseException>(call);
        Exception? innerException = exception.InnerException;
        Assert.NotNull(innerException);
        Assert.Equal($"Recipe modal response could not be successfully determined: {innerException!.Message}.", exception.Message);
    }

    private static void AssertAuthor(string expectedAuthorName, string expectedAuthorImageUrl, EmbedAuthor actualAuthor)
    {
        Assert.Equal(expectedAuthorName, actualAuthor.Name);
        Assert.Equal(expectedAuthorImageUrl, actualAuthor.IconUrl);
    }

    private static void AssertField(string expectedName, string expectedValue, EmbedField actualField)
    {
        Assert.Equal(expectedName, actualField.Name);
        Assert.Equal(expectedValue, actualField.Value);
        Assert.False(actualField.Inline);
    }

    private static IEnumerable<object[]> GetInvalidFieldCharacterLengths()
    {
        const string recipeIngredients = "My ingredients";
        const string recipeSteps = "My recipe steps";
        const string recipeNotes = "My notes";

        var invalidEntry = new string('x', EmbedFieldBuilder.MaxFieldValueLength + 1);

        yield return new object[]
        {
            invalidEntry,
            recipeSteps,
            recipeNotes
        };

        yield return new object[]
        {
            recipeIngredients,
            invalidEntry,
            recipeNotes
        };

        yield return new object[]
        {
            recipeIngredients,
            recipeSteps,
            invalidEntry
        };
    }
}