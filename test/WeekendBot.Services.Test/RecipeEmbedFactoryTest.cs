﻿// Copyright (C) 2022 Dennis Tang. All rights reserved.
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
using NSubstitute;
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
        EmbedAuthor embedAuthor = embed.Author.Value;
        AssertAuthor(authorName, authorImageUrl, embedAuthor);

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
        EmbedAuthor embedAuthor = embed.Author.Value;
        AssertAuthor(authorName, authorImageUrl, embedAuthor);

        Assert.Equal(recipeTitle, embed.Title);
        EmbedImage resultImage = embed.Image.Value;
        Assert.Equal(recipeImageUrl, resultImage.Url);

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
        EmbedAuthor embedAuthor = embed.Author.Value;
        AssertAuthor(authorName, authorImageUrl, embedAuthor);

        Assert.Equal(recipeTitle, embed.Title);
        EmbedImage resultImage = embed.Image.Value;
        Assert.Equal(recipeImageUrl, resultImage.Url);

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
        EmbedAuthor embedAuthor = embed.Author.Value;
        AssertAuthor(authorName, authorImageUrl, embedAuthor);

        Assert.Equal(recipeTitle, embed.Title);
        Assert.Null(embed.Image);

        Assert.Equal(3, embed.Fields.Length);
        AssertField("Ingredients", recipeIngredients, embed.Fields[0]);
        AssertField("Cooking steps", recipeSteps, embed.Fields[1]);
        AssertField("Additional notes", recipeNotes, embed.Fields[2]);
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
}