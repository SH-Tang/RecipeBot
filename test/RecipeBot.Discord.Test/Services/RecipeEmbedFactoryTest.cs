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
using Discord;
using RecipeBot.Discord.Services;
using RecipeBot.Domain.Models;
using RecipeBot.Domain.TestUtils;
using Xunit;

namespace RecipeBot.Discord.Test.Services;

public class RecipeEmbedFactoryTest
{
    private readonly RecipeDomainModelTestFactory domainTestFactory;

    public RecipeEmbedFactoryTest()
    {
        domainTestFactory = new RecipeDomainModelTestFactory(new RecipeDomainModelTestFactory.ConstructionProperties
        {
            MaxAuthorNameLength = EmbedAuthorBuilder.MaxAuthorNameLength,
            MaxTitleLength = EmbedBuilder.MaxTitleLength,
            MaxFieldNameLength = EmbedFieldBuilder.MaxFieldNameLength,
            MaxFieldDataLength = EmbedFieldBuilder.MaxFieldValueLength
        });
    }

    [Fact]
    public void Basic_recipe_should_return_embed_without_image_and_fields()
    {
        // Setup
        RecipeDomainEntity recipeDomainEntity = domainTestFactory.Create();

        // Call
        Embed embed = RecipeEmbedFactory.Create(recipeDomainEntity);

        // Assert
        Assert.Equal(recipeDomainEntity.Title, embed.Title);
        Assert.Null(embed.Image);

        AssertAuthor(recipeDomainEntity.AuthorEntity, embed.Author);
        AssertFields(recipeDomainEntity.RecipeFieldEntities, embed.Fields);
    }

    [Fact]
    public void Recipe_with_fields_and_image_should_return_embed_with_fields_and_image()
    {
        // Setup
        RecipeDomainEntity recipeDomainEntity = domainTestFactory.CreateWithImageAndFields();

        // Call
        Embed embed = RecipeEmbedFactory.Create(recipeDomainEntity);

        // Assert
        Assert.Equal(recipeDomainEntity.Title, embed.Title);

        EmbedImage? embedImage = embed.Image;
        Assert.NotNull(embedImage);
        Assert.Equal(recipeDomainEntity.RecipeImageUrl, embedImage!.Value.Url);

        AssertAuthor(recipeDomainEntity.AuthorEntity, embed.Author);
        AssertFields(recipeDomainEntity.RecipeFieldEntities, embed.Fields);
    }

    [Fact]
    public void Recipe_with_image_should_return_embed_with_image()
    {
        // Setup
        RecipeDomainEntity recipeDomainEntity = domainTestFactory.CreateWithImage();

        // Call
        Embed embed = RecipeEmbedFactory.Create(recipeDomainEntity);

        // Assert
        Assert.Equal(recipeDomainEntity.Title, embed.Title);

        EmbedImage? embedImage = embed.Image;
        Assert.NotNull(embedImage);
        Assert.Equal(recipeDomainEntity.RecipeImageUrl, embedImage!.Value.Url);

        AssertAuthor(recipeDomainEntity.AuthorEntity, embed.Author);
        AssertFields(recipeDomainEntity.RecipeFieldEntities, embed.Fields);
    }

    [Fact]
    public void Recipe_with_fields_should_return_embed_with_fields()
    {
        // Setup
        RecipeDomainEntity recipeDomainEntity = domainTestFactory.CreateWithFields();

        // Call
        Embed embed = RecipeEmbedFactory.Create(recipeDomainEntity);

        // Assert
        Assert.Equal(recipeDomainEntity.Title, embed.Title);

        EmbedImage? embedImage = embed.Image;
        Assert.Null(embedImage);

        AssertAuthor(recipeDomainEntity.AuthorEntity, embed.Author);
        AssertFields(recipeDomainEntity.RecipeFieldEntities, embed.Fields);
    }

    private static void AssertAuthor(AuthorDomainEntity authorData, EmbedAuthor? actualAuthor)
    {
        Assert.NotNull(actualAuthor);
        Assert.Equal(authorData.AuthorName, actualAuthor!.Value.Name);
        Assert.Equal(authorData.AuthorImageUrl, actualAuthor.Value.IconUrl);
    }

    private static void AssertFields(IEnumerable<RecipeFieldDomainEntity> fieldDomainEntities, IEnumerable<EmbedField> embedFields)
    {
        int nrOfFieldDomainEntities = fieldDomainEntities.Count();
        Assert.Equal(nrOfFieldDomainEntities, embedFields.Count());
        for (var i = 0; i < nrOfFieldDomainEntities; i++)
        {
            AssertField(fieldDomainEntities.ElementAt(i), embedFields.ElementAt(i));
        }
    }

    private static void AssertField(RecipeFieldDomainEntity domainEntity, EmbedField actualField)
    {
        Assert.Equal(domainEntity.FieldName, actualField.Name);
        Assert.Equal(domainEntity.FieldData, actualField.Value);
        Assert.False(actualField.Inline);
    }
}