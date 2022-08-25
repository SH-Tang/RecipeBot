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
using System.Linq;
using NSubstitute;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Entities;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Factories;
using RecipeBot.TestUtils;
using Xunit;

namespace RecipeBot.Domain.Test.Factories;

public class RecipeDomainEntityFactoryTest
{
    [Theory]
    [ClassData(typeof(NullOrWhitespacesStringValueGenerator))]
    public void Recipe_without_image_and_notes_returns_entity_without_image_and_notes(string notes)
    {
        // Setup
        var recipeCharacterLimitProvider = Substitute.For<IRecipeDomainEntityCharacterLimitProvider>();
        recipeCharacterLimitProvider.MaximumTitleLength.Returns(10);
        recipeCharacterLimitProvider.MaximumRecipeLength.Returns(int.MaxValue);
        recipeCharacterLimitProvider.MaximumAuthorNameLength.Returns(10);
        recipeCharacterLimitProvider.MaximumFieldNameLength.Returns(int.MaxValue);
        recipeCharacterLimitProvider.MaximumFieldDataLength.Returns(20);

        RecipeData recipeData = CreateRecipeData(recipeCharacterLimitProvider);
        recipeData.AdditionalNotes = notes;

        var factory = new RecipeDomainEntityFactory(recipeCharacterLimitProvider);

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
        recipeCharacterLimitProvider.MaximumAuthorNameLength.Returns(10);
        recipeCharacterLimitProvider.MaximumFieldNameLength.Returns(int.MaxValue);
        recipeCharacterLimitProvider.MaximumFieldDataLength.Returns(20);

        const string imageUrl = "http://www.recipeBotImage.com";
        RecipeData recipeData = CreateRecipeData(recipeCharacterLimitProvider);
        recipeData.ImageUrl = imageUrl;

        var factory = new RecipeDomainEntityFactory(recipeCharacterLimitProvider);

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
        recipeCharacterLimitProvider.MaximumAuthorNameLength.Returns(10);
        recipeCharacterLimitProvider.MaximumFieldNameLength.Returns(int.MaxValue);
        recipeCharacterLimitProvider.MaximumFieldDataLength.Returns(20);

        RecipeData recipeData = CreateRecipeData(recipeCharacterLimitProvider);
        recipeData.AdditionalNotes = new string('%', recipeCharacterLimitProvider.MaximumFieldDataLength);

        var factory = new RecipeDomainEntityFactory(recipeCharacterLimitProvider);

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
        recipeCharacterLimitProvider.MaximumAuthorNameLength.Returns(10);
        recipeCharacterLimitProvider.MaximumFieldNameLength.Returns(int.MaxValue);
        recipeCharacterLimitProvider.MaximumFieldDataLength.Returns(20);

        const string imageUrl = "http://www.recipeBotImage.com";
        RecipeData recipeData = CreateRecipeData(recipeCharacterLimitProvider);
        recipeData.ImageUrl = imageUrl;
        recipeData.AdditionalNotes = new string('%', recipeCharacterLimitProvider.MaximumFieldDataLength);

        var factory = new RecipeDomainEntityFactory(recipeCharacterLimitProvider);

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
        recipeCharacterLimitProvider.MaximumAuthorNameLength.Returns(10);
        recipeCharacterLimitProvider.MaximumFieldNameLength.Returns(int.MaxValue);
        recipeCharacterLimitProvider.MaximumFieldDataLength.Returns(20);

        RecipeData recipeData = CreateRecipeData(recipeCharacterLimitProvider, 1);

        var factory = new RecipeDomainEntityFactory(recipeCharacterLimitProvider);

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
        recipeCharacterLimitProvider.MaximumAuthorNameLength.Returns(10);
        recipeCharacterLimitProvider.MaximumFieldNameLength.Returns(int.MaxValue);
        recipeCharacterLimitProvider.MaximumFieldDataLength.Returns(20);

        RecipeData recipeData = CreateRecipeData(recipeCharacterLimitProvider, -characterOffset);

        var factory = new RecipeDomainEntityFactory(recipeCharacterLimitProvider);

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
        recipeCharacterLimitProvider.MaximumAuthorNameLength.Returns(10);
        recipeCharacterLimitProvider.MaximumFieldNameLength.Returns(30);
        recipeCharacterLimitProvider.MaximumFieldDataLength.Returns(20);

        RecipeData recipeData = CreateRecipeData(recipeCharacterLimitProvider);

        var factory = new RecipeDomainEntityFactory(recipeCharacterLimitProvider);

        // Precondition
        int totalCharacterLength = recipeCharacterLimitProvider.MaximumAuthorNameLength +
                                   recipeCharacterLimitProvider.MaximumTitleLength +
                                   recipeCharacterLimitProvider.MaximumFieldDataLength;
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
        recipeCharacterLimitProvider.MaximumAuthorNameLength.Returns(10);
        recipeCharacterLimitProvider.MaximumFieldNameLength.Returns(int.MaxValue);
        recipeCharacterLimitProvider.MaximumFieldDataLength.Returns(20);

        RecipeData recipeData = CreateRecipeData(recipeCharacterLimitProvider);
        recipeData.ImageUrl = "invalid image url";

        var factory = new RecipeDomainEntityFactory(recipeCharacterLimitProvider);

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

    private static RecipeData CreateRecipeData(IRecipeDomainEntityCharacterLimitProvider limitProvider,
                                               int nrOfTitleCharactersOffSet)
    {
        var authorData = new AuthorData(new string('x', limitProvider.MaximumAuthorNameLength), "http://www.recipeBotImage.com");
        return new RecipeData(authorData, new string('+', limitProvider.MaximumTitleLength + nrOfTitleCharactersOffSet),
                              new string('o', limitProvider.MaximumFieldDataLength), new string('#', limitProvider.MaximumFieldDataLength));
    }

    private static RecipeData CreateRecipeData(IRecipeDomainEntityCharacterLimitProvider limitProvider)
    {
        var authorData = new AuthorData(new string('x', limitProvider.MaximumAuthorNameLength), "http://www.recipeBotImage.com");
        return new RecipeData(authorData, new string('+', limitProvider.MaximumTitleLength),
                              new string('o', limitProvider.MaximumFieldDataLength), new string('#', limitProvider.MaximumFieldDataLength));
    }
}