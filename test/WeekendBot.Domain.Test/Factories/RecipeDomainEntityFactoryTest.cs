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
using System.Linq;
using NSubstitute;
using WeekendBot.Domain.Data;
using WeekendBot.Domain.Entities;
using WeekendBot.Domain.Exceptions;
using WeekendBot.Domain.Factories;
using Xunit;

namespace WeekendBot.Domain.Test.Factories;

public class RecipeDomainEntityFactoryTest
{
    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData(null)]
    public void Recipe_without_image_and_notes_returns_entity_without_image_and_notes(string notes)
    {
        // Setup
        var recipeCharacterLimitProvider = Substitute.For<IRecipeDomainEntityCharacterLimitProvider>();
        recipeCharacterLimitProvider.MaximumTitleLength.Returns(10);
        recipeCharacterLimitProvider.MaximumRecipeLength.Returns(int.MaxValue);

        var authorCharacterLimitProvider = Substitute.For<IAuthorDomainEntityCharacterLimitProvider>();
        authorCharacterLimitProvider.MaximumAuthorNameLength.Returns(10);

        var fieldCharacterLimitProvider = Substitute.For<IRecipeFieldDomainEntityCharacterLimitProvider>();
        fieldCharacterLimitProvider.MaximumFieldNameLength.Returns(int.MaxValue);
        fieldCharacterLimitProvider.MaximumFieldDataLength.Returns(20);

        RecipeData recipeData = CreateRecipeData(authorCharacterLimitProvider.MaximumAuthorNameLength,
                                                 recipeCharacterLimitProvider.MaximumTitleLength,
                                                 fieldCharacterLimitProvider.MaximumFieldDataLength);
        recipeData.AdditionalNotes = notes;

        var factory = new RecipeDomainEntityFactory(recipeCharacterLimitProvider, authorCharacterLimitProvider, fieldCharacterLimitProvider);

        // Call
        RecipeDomainEntity entity = factory.Create(recipeData);

        // Assert
        Assert.Null(entity.RecipeImageUrl);

        AssertAuthorEntity(recipeData.AuthorData, entity.AuthorEntity);
        Assert.Equal(2, entity.RecipeFieldEntities.Count());
        AssertMandatoryFields(recipeData, entity.RecipeFieldEntities);
    }

    [Fact]
    public void Recipe_with_image_returns_entity_with_image()
    {
        // Setup
        var recipeCharacterLimitProvider = Substitute.For<IRecipeDomainEntityCharacterLimitProvider>();
        recipeCharacterLimitProvider.MaximumTitleLength.Returns(10);
        recipeCharacterLimitProvider.MaximumRecipeLength.Returns(int.MaxValue);

        var authorCharacterLimitProvider = Substitute.For<IAuthorDomainEntityCharacterLimitProvider>();
        authorCharacterLimitProvider.MaximumAuthorNameLength.Returns(10);

        var fieldCharacterLimitProvider = Substitute.For<IRecipeFieldDomainEntityCharacterLimitProvider>();
        fieldCharacterLimitProvider.MaximumFieldNameLength.Returns(int.MaxValue);
        fieldCharacterLimitProvider.MaximumFieldDataLength.Returns(20);

        const string imageUrl = "http://www.bing.com";
        RecipeData recipeData = CreateRecipeData(authorCharacterLimitProvider.MaximumAuthorNameLength,
                                                 recipeCharacterLimitProvider.MaximumTitleLength,
                                                 fieldCharacterLimitProvider.MaximumFieldDataLength);
        recipeData.ImageUrl = imageUrl;

        var factory = new RecipeDomainEntityFactory(recipeCharacterLimitProvider, authorCharacterLimitProvider, fieldCharacterLimitProvider);

        // Call
        RecipeDomainEntity entity = factory.Create(recipeData);

        // Assert
        Assert.Equal(imageUrl, entity.RecipeImageUrl);

        AssertAuthorEntity(recipeData.AuthorData, entity.AuthorEntity);
        Assert.Equal(2, entity.RecipeFieldEntities.Count());
        AssertMandatoryFields(recipeData, entity.RecipeFieldEntities);
    }

    [Fact]
    public void Recipe_with_notes_returns_entity_with_notes()
    {
        // Setup
        var recipeCharacterLimitProvider = Substitute.For<IRecipeDomainEntityCharacterLimitProvider>();
        recipeCharacterLimitProvider.MaximumTitleLength.Returns(10);
        recipeCharacterLimitProvider.MaximumRecipeLength.Returns(int.MaxValue);

        var authorCharacterLimitProvider = Substitute.For<IAuthorDomainEntityCharacterLimitProvider>();
        authorCharacterLimitProvider.MaximumAuthorNameLength.Returns(10);

        var fieldCharacterLimitProvider = Substitute.For<IRecipeFieldDomainEntityCharacterLimitProvider>();
        fieldCharacterLimitProvider.MaximumFieldNameLength.Returns(int.MaxValue);
        fieldCharacterLimitProvider.MaximumFieldDataLength.Returns(20);

        RecipeData recipeData = CreateRecipeData(authorCharacterLimitProvider.MaximumAuthorNameLength,
                                                 recipeCharacterLimitProvider.MaximumTitleLength,
                                                 fieldCharacterLimitProvider.MaximumFieldDataLength);
        recipeData.AdditionalNotes = new string('%', fieldCharacterLimitProvider.MaximumFieldDataLength);

        var factory = new RecipeDomainEntityFactory(recipeCharacterLimitProvider, authorCharacterLimitProvider, fieldCharacterLimitProvider);

        // Call
        RecipeDomainEntity entity = factory.Create(recipeData);

        // Assert
        Assert.Null(entity.RecipeImageUrl);

        AssertAuthorEntity(recipeData.AuthorData, entity.AuthorEntity);

        IEnumerable<RecipeFieldDomainEntity> recipeFieldEntities = entity.RecipeFieldEntities;
        Assert.Equal(3, recipeFieldEntities.Count());
        AssertMandatoryFields(recipeData, recipeFieldEntities);
        RecipeFieldDomainEntity cookingStepsDomainFieldEntity = recipeFieldEntities.ElementAt(2);
        Assert.Equal("Additional notes", cookingStepsDomainFieldEntity.FieldName);
        Assert.Equal(recipeData.AdditionalNotes, cookingStepsDomainFieldEntity.FieldData);
    }

    [Fact]
    public void Recipe_with_image_and_notes_returns_entity_with_image_and_notes()
    {
        // Setup
        var recipeCharacterLimitProvider = Substitute.For<IRecipeDomainEntityCharacterLimitProvider>();
        recipeCharacterLimitProvider.MaximumTitleLength.Returns(10);
        recipeCharacterLimitProvider.MaximumRecipeLength.Returns(int.MaxValue);

        var authorCharacterLimitProvider = Substitute.For<IAuthorDomainEntityCharacterLimitProvider>();
        authorCharacterLimitProvider.MaximumAuthorNameLength.Returns(10);

        var fieldCharacterLimitProvider = Substitute.For<IRecipeFieldDomainEntityCharacterLimitProvider>();
        fieldCharacterLimitProvider.MaximumFieldNameLength.Returns(int.MaxValue);
        fieldCharacterLimitProvider.MaximumFieldDataLength.Returns(20);

        const string imageUrl = "http://www.bing.com";
        RecipeData recipeData = CreateRecipeData(authorCharacterLimitProvider.MaximumAuthorNameLength,
                                                 recipeCharacterLimitProvider.MaximumTitleLength,
                                                 fieldCharacterLimitProvider.MaximumFieldDataLength);
        recipeData.ImageUrl = imageUrl;
        recipeData.AdditionalNotes = new string('%', fieldCharacterLimitProvider.MaximumFieldDataLength);

        var factory = new RecipeDomainEntityFactory(recipeCharacterLimitProvider, authorCharacterLimitProvider, fieldCharacterLimitProvider);

        // Call
        RecipeDomainEntity entity = factory.Create(recipeData);

        // Assert
        Assert.Equal(recipeData.RecipeTitle, entity.Title);
        Assert.Equal(recipeData.ImageUrl, entity.RecipeImageUrl);

        IEnumerable<RecipeFieldDomainEntity> recipeFieldEntities = entity.RecipeFieldEntities;
        Assert.Equal(3, recipeFieldEntities.Count());
        AssertMandatoryFields(recipeData, recipeFieldEntities);
        RecipeFieldDomainEntity cookingStepsDomainFieldEntity = recipeFieldEntities.ElementAt(2);
        Assert.Equal("Additional notes", cookingStepsDomainFieldEntity.FieldName);
        Assert.Equal(recipeData.AdditionalNotes, cookingStepsDomainFieldEntity.FieldData);
    }

    [Fact]
    public void Recipe_with_title_and_invalid_character_length_throws_exception()
    {
        // Setup
        var recipeCharacterLimitProvider = Substitute.For<IRecipeDomainEntityCharacterLimitProvider>();
        recipeCharacterLimitProvider.MaximumTitleLength.Returns(10);
        recipeCharacterLimitProvider.MaximumRecipeLength.Returns(int.MaxValue);

        var authorCharacterLimitProvider = Substitute.For<IAuthorDomainEntityCharacterLimitProvider>();
        authorCharacterLimitProvider.MaximumAuthorNameLength.Returns(10);

        var fieldCharacterLimitProvider = Substitute.For<IRecipeFieldDomainEntityCharacterLimitProvider>();
        fieldCharacterLimitProvider.MaximumFieldNameLength.Returns(int.MaxValue);
        fieldCharacterLimitProvider.MaximumFieldDataLength.Returns(20);

        RecipeData recipeData = CreateRecipeData(authorCharacterLimitProvider.MaximumAuthorNameLength,
                                                 recipeCharacterLimitProvider.MaximumTitleLength + 1,
                                                 fieldCharacterLimitProvider.MaximumFieldDataLength);

        var factory = new RecipeDomainEntityFactory(recipeCharacterLimitProvider, authorCharacterLimitProvider, fieldCharacterLimitProvider);

        // Call
        Action call = () => factory.Create(recipeData);

        // Assert
        var exception = Assert.Throws<DomainEntityCreateException>(call);
        string expectedMessage = $"RecipeTitle must be less or equal to {recipeCharacterLimitProvider.MaximumTitleLength} characters.";
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Theory]
    [InlineData(8)]
    [InlineData(0)]
    [InlineData(1)]
    public void Recipe_with_valid_title_returns_entity_with_title(int characterOffset)
    {
        // Setup
        var recipeCharacterLimitProvider = Substitute.For<IRecipeDomainEntityCharacterLimitProvider>();
        recipeCharacterLimitProvider.MaximumTitleLength.Returns(10);
        recipeCharacterLimitProvider.MaximumRecipeLength.Returns(int.MaxValue);

        var authorCharacterLimitProvider = Substitute.For<IAuthorDomainEntityCharacterLimitProvider>();
        authorCharacterLimitProvider.MaximumAuthorNameLength.Returns(10);

        var fieldCharacterLimitProvider = Substitute.For<IRecipeFieldDomainEntityCharacterLimitProvider>();
        fieldCharacterLimitProvider.MaximumFieldNameLength.Returns(int.MaxValue);
        fieldCharacterLimitProvider.MaximumFieldDataLength.Returns(20);

        RecipeData recipeData = CreateRecipeData(authorCharacterLimitProvider.MaximumAuthorNameLength,
                                                 recipeCharacterLimitProvider.MaximumTitleLength - characterOffset,
                                                 fieldCharacterLimitProvider.MaximumFieldDataLength);

        var factory = new RecipeDomainEntityFactory(recipeCharacterLimitProvider, authorCharacterLimitProvider, fieldCharacterLimitProvider);

        // Call
        RecipeDomainEntity entity = factory.Create(recipeData);

        // Assert
        Assert.Equal(recipeData.RecipeTitle, entity.Title);
    }

    [Fact]
    public void Recipe_with_data_exceeding_total_recipe_length_throws_exception()
    {
        // Setup
        var recipeCharacterLimitProvider = Substitute.For<IRecipeDomainEntityCharacterLimitProvider>();
        recipeCharacterLimitProvider.MaximumTitleLength.Returns(10);
        recipeCharacterLimitProvider.MaximumRecipeLength.Returns(1);

        var authorCharacterLimitProvider = Substitute.For<IAuthorDomainEntityCharacterLimitProvider>();
        authorCharacterLimitProvider.MaximumAuthorNameLength.Returns(10);

        var fieldCharacterLimitProvider = Substitute.For<IRecipeFieldDomainEntityCharacterLimitProvider>();
        fieldCharacterLimitProvider.MaximumFieldNameLength.Returns(30);
        fieldCharacterLimitProvider.MaximumFieldDataLength.Returns(20);

        RecipeData recipeData = CreateRecipeData(authorCharacterLimitProvider.MaximumAuthorNameLength,
                                                 recipeCharacterLimitProvider.MaximumTitleLength,
                                                 fieldCharacterLimitProvider.MaximumFieldDataLength);

        var factory = new RecipeDomainEntityFactory(recipeCharacterLimitProvider, authorCharacterLimitProvider, fieldCharacterLimitProvider);

        // Precondition
        int totalCharacterLength = authorCharacterLimitProvider.MaximumAuthorNameLength +
                                   recipeCharacterLimitProvider.MaximumTitleLength +
                                   fieldCharacterLimitProvider.MaximumFieldDataLength;
        Assert.True(totalCharacterLength > recipeCharacterLimitProvider.MaximumRecipeLength);

        // Call                    
        Action call = () => factory.Create(recipeData);

        // Assert
        var exception = Assert.Throws<DomainEntityCreateException>(call);
        string expectedMessage = $"recipeData must be less or equal to {recipeCharacterLimitProvider.MaximumRecipeLength} characters.";
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void Recipe_with_invalid_image_url_throws_exception()
    {
        // Setup
        var recipeCharacterLimitProvider = Substitute.For<IRecipeDomainEntityCharacterLimitProvider>();
        recipeCharacterLimitProvider.MaximumTitleLength.Returns(10);
        recipeCharacterLimitProvider.MaximumRecipeLength.Returns(int.MaxValue);

        var authorCharacterLimitProvider = Substitute.For<IAuthorDomainEntityCharacterLimitProvider>();
        authorCharacterLimitProvider.MaximumAuthorNameLength.Returns(10);

        var fieldCharacterLimitProvider = Substitute.For<IRecipeFieldDomainEntityCharacterLimitProvider>();
        fieldCharacterLimitProvider.MaximumFieldNameLength.Returns(int.MaxValue);
        fieldCharacterLimitProvider.MaximumFieldDataLength.Returns(20);

        RecipeData recipeData = CreateRecipeData(authorCharacterLimitProvider.MaximumAuthorNameLength,
                                                 recipeCharacterLimitProvider.MaximumTitleLength,
                                                 fieldCharacterLimitProvider.MaximumFieldDataLength);
        recipeData.ImageUrl = "invalid image url";

        var factory = new RecipeDomainEntityFactory(recipeCharacterLimitProvider, authorCharacterLimitProvider, fieldCharacterLimitProvider);

        // Call                    
        Action call = () => factory.Create(recipeData);

        // Assert
        Assert.Throws<DomainEntityCreateException>(call);
    }

    private static void AssertAuthorEntity(AuthorData data, AuthorDomainEntity domainEntity)
    {
        Assert.Equal(data.AuthorName, domainEntity.AuthorName);
        Assert.Equal(data.AuthorImageUrl, domainEntity.AuthorImageUrl);
    }

    private static void AssertMandatoryFields(RecipeData data, IEnumerable<RecipeFieldDomainEntity> domainEntities)
    {
        RecipeFieldDomainEntity ingredientsDomainFieldEntity = domainEntities.First();
        Assert.Equal("Ingredients", ingredientsDomainFieldEntity.FieldName);
        Assert.Equal(data.RecipeIngredients, ingredientsDomainFieldEntity.FieldData);

        RecipeFieldDomainEntity cookingStepsDomainFieldEntity = domainEntities.ElementAt(1);
        Assert.Equal("Cooking steps", cookingStepsDomainFieldEntity.FieldName);
        Assert.Equal(data.CookingSteps, cookingStepsDomainFieldEntity.FieldData);
    }

    private static RecipeData CreateRecipeData(int maxAuthorNameCharacters, int maxTitleCharacters, int maxFieldCharacters)
    {
        var authorData = new AuthorData(new string('x', maxAuthorNameCharacters), "http://www.google.com");
        return new RecipeData(authorData, new string('+', maxTitleCharacters),
                              new string('o', maxFieldCharacters), new string('#', maxTitleCharacters));
    }
}