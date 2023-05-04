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

using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Discord;
using Discord.Common.Providers;
using Discord.Common.TestUtils;
using FluentAssertions;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Models;
using RecipeBot.Domain.TestUtils;
using RecipeBot.Services;
using Xunit;

namespace RecipeBot.Test.Services;

public class RecipeEmbedFactoryTest
{
    private readonly RecipeModelTestBuilder modelBuilder;

    public RecipeEmbedFactoryTest()
    {
        modelBuilder = new RecipeModelTestBuilder(new RecipeModelTestBuilder.ConstructionProperties
        {
            MaxAuthorNameLength = EmbedAuthorBuilder.MaxAuthorNameLength,
            MaxTitleLength = EmbedBuilder.MaxTitleLength,
            MaxFieldNameLength = EmbedFieldBuilder.MaxFieldNameLength,
            MaxFieldDataLength = EmbedFieldBuilder.MaxFieldValueLength
        });
    }

    [Theory]
    [MemberData(nameof(GetRecipeCategoriesAndColor))]
    public void Basic_recipe_with_category_returns_embed_with_color(
        RecipeCategory category, Color expectedColor)
    {
        // Setup
        RecipeModel recipeModel = modelBuilder.SetCategory(category)
                                              .Build();

        // Call
        Embed embed = RecipeEmbedFactory.Create(recipeModel, UserDataTestFactory.CreateFullyConfigured());

        // Assert
        embed.Color.Should().Be(expectedColor);
    }

    [Fact]
    public void Basic_recipe_should_return_embed_without_image_and_fields()
    {
        // Setup
        RecipeModel recipeModel = modelBuilder.Build();
        UserData author = UserDataTestFactory.CreateFullyConfigured();

        // Call
        Embed embed = RecipeEmbedFactory.Create(recipeModel, author);

        // Assert
        embed.Title.Should().Be(recipeModel.Title);
        embed.Image.Should().BeNull();

        AssertAuthor(author, embed.Author);
        AssertFields(recipeModel.RecipeFields, embed.Fields);

        embed.Footer.Should().BeNull();
    }

    [Fact]
    public void Recipe_with_all_data_should_return_embed_with_fields_image_and_footer()
    {
        // Setup
        var fixture = new Fixture();
        var category = fixture.Create<RecipeCategory>();
        RecipeModel recipeModel = modelBuilder.SetCategory(category)
                                              .AddImage()
                                              .AddTags(new[]
                                              {
                                                  "Tag1",
                                                  "Tag2"
                                              })
                                              .AddFields(3)
                                              .Build();
        UserData author = UserDataTestFactory.CreateFullyConfigured();

        // Call
        Embed embed = RecipeEmbedFactory.Create(recipeModel, author);

        // Assert
        embed.Title.Should().Be(recipeModel.Title);
        embed.Image.Should().NotBeNull().And.Match<EmbedImage>(s => s.Url == recipeModel.RecipeImageUrl);

        AssertAuthor(author, embed.Author);
        AssertFields(recipeModel.RecipeFields, embed.Fields);

        var expectedFooterText = $"{TagTestHelper.CategoryMapping[category]}, Tag1, Tag2";
        EmbedFooter? embedFooter = embed.Footer;
        embedFooter.Should().NotBeNull().And.Match<EmbedFooter>(s => s.Text == expectedFooterText);
    }

    [Fact]
    public void Recipe_with_image_should_return_embed_with_image()
    {
        // Setup
        RecipeModel recipeModel = modelBuilder.AddImage()
                                              .Build();
        UserData author = UserDataTestFactory.CreateFullyConfigured();

        // Call
        Embed embed = RecipeEmbedFactory.Create(recipeModel, author);

        // Assert
        embed.Title.Should().Be(recipeModel.Title);
        embed.Image.Should().NotBeNull().And.Match<EmbedImage>(s => s.Url == recipeModel.RecipeImageUrl);

        AssertAuthor(author, embed.Author);
        AssertFields(recipeModel.RecipeFields, embed.Fields);

        embed.Footer.Should().BeNull();
    }

    [Fact]
    public void Recipe_with_fields_should_return_embed_with_fields()
    {
        // Setup
        RecipeModel recipeModel = modelBuilder.AddFields(3)
                                              .Build();
        UserData author = UserDataTestFactory.CreateFullyConfigured();

        // Call
        Embed embed = RecipeEmbedFactory.Create(recipeModel, author);

        // Assert
        embed.Title.Should().Be(recipeModel.Title);
        embed.Image.Should().BeNull();

        AssertAuthor(author, embed.Author);
        AssertFields(recipeModel.RecipeFields, embed.Fields);

        embed.Footer.Should().BeNull();
    }

    [Fact]
    public void Recipe_with_tags_should_return_embed_with_tags()
    {
        // Setup
        var fixture = new Fixture();
        var category = fixture.Create<RecipeCategory>();
        RecipeModel recipeModel = modelBuilder.SetCategory(category)
                                              .AddTags(new[]
                                              {
                                                  "Tag1",
                                                  "Tag2"
                                              })
                                              .Build();
        UserData author = UserDataTestFactory.CreateFullyConfigured();

        // Call
        Embed embed = RecipeEmbedFactory.Create(recipeModel, author);

        // Assert
        embed.Title.Should().Be(recipeModel.Title);
        embed.Image.Should().BeNull();

        AssertAuthor(author, embed.Author);
        AssertFields(recipeModel.RecipeFields, embed.Fields);

        var expectedFooterText = $"{TagTestHelper.CategoryMapping[category]}, Tag1, Tag2";
        EmbedFooter? embedFooter = embed.Footer;
        embedFooter.Should().NotBeNull().And.Match<EmbedFooter>(s => s.Text == expectedFooterText);
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

    private static void AssertAuthor(UserData authorData, EmbedAuthor? actualAuthor)
    {
        actualAuthor.Should().NotBeNull().And.BeEquivalentTo(
            authorData,
            options => options.ExcludingMissingMembers()
                              .WithMapping<EmbedAuthor>(e => e.Username, s => s.Name)
                              .WithMapping<EmbedAuthor>(e => e.UserImageUrl, s => s.IconUrl));
    }

    private static void AssertFields(IEnumerable<RecipeFieldModel> recipeFields, IEnumerable<EmbedField> embedFields)
    {
        if (!recipeFields.Any())
        {
            embedFields.Should().BeEmpty();
        }
        else
        {
            embedFields.Should().NotBeNull().And.BeEquivalentTo(
                recipeFields,
                options => options.WithStrictOrdering()
                                  .ExcludingMissingMembers()
                                  .WithMapping<EmbedField>(e => e.FieldName, s => s.Name)
                                  .WithMapping<EmbedField>(e => e.FieldData, s => s.Value));
            embedFields.Should().AllSatisfy(s => s.Inline.Should().BeFalse());
        }
    }
}