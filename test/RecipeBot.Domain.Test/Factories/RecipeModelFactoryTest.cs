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
using AutoFixture;
using NSubstitute;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Factories;
using RecipeBot.Domain.Models;
using RecipeBot.Domain.TestUtils;
using RecipeBot.TestUtils;
using Xunit;

namespace RecipeBot.Domain.Test.Factories;

public class RecipeModelFactoryTest
{
    [Theory]
    [ClassData(typeof(NullOrWhitespacesStringValueGenerator))]
    public void Recipe_without_optional_data_returns_model_without_optional_data(string emptyValueString)
    {
        // Setup
        IRecipeModelCharacterLimitProvider recipeCharacterLimitProvider = CreateDefaultRecipeCharacterLimitProvider();

        RecipeData recipeData = CreateRecipeData(recipeCharacterLimitProvider);
        recipeData.AdditionalNotes = emptyValueString;
        recipeData.Tags = emptyValueString;

        var factory = new RecipeModelFactory(recipeCharacterLimitProvider);

        // Call
        RecipeModel model = factory.Create(recipeData);

        // Assert
        Assert.Null(model.RecipeImageUrl);

        AssertAuthor(recipeData.AuthorData, model.Author);
        Assert.Equal(2, model.RecipeFields.Count());
        AssertMandatoryFields(recipeData, model.RecipeFields);
        AssertTags(recipeData, model.RecipeTags);
    }

    [Fact]
    public void Recipe_with_image_returns_model_with_image()
    {
        // Setup
        IRecipeModelCharacterLimitProvider recipeCharacterLimitProvider = CreateDefaultRecipeCharacterLimitProvider();

        const string imageUrl = "http://www.recipeBotImage.com";
        RecipeData recipeData = CreateRecipeData(recipeCharacterLimitProvider);
        recipeData.ImageUrl = imageUrl;

        var factory = new RecipeModelFactory(recipeCharacterLimitProvider);

        // Call
        RecipeModel model = factory.Create(recipeData);

        // Assert
        Assert.Equal(imageUrl, model.RecipeImageUrl);

        AssertAuthor(recipeData.AuthorData, model.Author);
        Assert.Equal(2, model.RecipeFields.Count());
        AssertMandatoryFields(recipeData, model.RecipeFields);
        AssertTags(recipeData, model.RecipeTags);
    }

    [Fact]
    public void Recipe_with_tags_returns_model_with_tags()
    {
        // Setup
        IRecipeModelCharacterLimitProvider recipeCharacterLimitProvider = CreateDefaultRecipeCharacterLimitProvider();

        RecipeData recipeData = CreateRecipeData(recipeCharacterLimitProvider);
        recipeData.Tags = "Tag1, TAG1, tag1, tag    1,      tag1, tag1      , tag2";

        var factory = new RecipeModelFactory(recipeCharacterLimitProvider);

        // Call
        RecipeModel model = factory.Create(recipeData);

        // Assert
        Assert.Equal(recipeData.RecipeTitle, model.Title);
        Assert.Null(model.RecipeImageUrl);

        IEnumerable<RecipeFieldModel> recipeFields = model.RecipeFields;
        Assert.Equal(2, recipeFields.Count());
        AssertMandatoryFields(recipeData, recipeFields);

        AssertTags(recipeData, model.RecipeTags);
    }

    [Fact]
    public void Recipe_with_notes_returns_model_with_notes()
    {
        // Setup
        IRecipeModelCharacterLimitProvider recipeCharacterLimitProvider = CreateDefaultRecipeCharacterLimitProvider();

        RecipeData recipeData = CreateRecipeData(recipeCharacterLimitProvider);
        recipeData.AdditionalNotes = new string('%', recipeCharacterLimitProvider.MaximumFieldDataLength);

        var factory = new RecipeModelFactory(recipeCharacterLimitProvider);

        // Call
        RecipeModel model = factory.Create(recipeData);

        // Assert
        Assert.Null(model.RecipeImageUrl);

        AssertAuthor(recipeData.AuthorData, model.Author);

        IEnumerable<RecipeFieldModel> recipeFields = model.RecipeFields;
        Assert.Equal(3, recipeFields.Count());
        AssertMandatoryFields(recipeData, recipeFields);
        RecipeFieldModel additionalNotesField = recipeFields.ElementAt(2);
        Assert.Equal("Additional notes", additionalNotesField.FieldName);
        Assert.Equal(recipeData.AdditionalNotes, additionalNotesField.FieldData);

        AssertTags(recipeData, model.RecipeTags);
    }

    [Fact]
    public void Recipe_with_all_optional_data_returns_model_with_all_data()
    {
        // Setup
        IRecipeModelCharacterLimitProvider recipeCharacterLimitProvider = CreateDefaultRecipeCharacterLimitProvider();

        const string imageUrl = "http://www.recipeBotImage.com";
        RecipeData recipeData = CreateRecipeData(recipeCharacterLimitProvider);
        recipeData.ImageUrl = imageUrl;
        recipeData.AdditionalNotes = new string('%', recipeCharacterLimitProvider.MaximumFieldDataLength);
        recipeData.Tags = "Tag1, TAG1, tag1, tag    1,      tag1, tag1      , tag2";

        var factory = new RecipeModelFactory(recipeCharacterLimitProvider);

        // Call
        RecipeModel model = factory.Create(recipeData);

        // Assert
        Assert.Equal(recipeData.RecipeTitle, model.Title);
        Assert.Equal(recipeData.ImageUrl, model.RecipeImageUrl);

        IEnumerable<RecipeFieldModel> recipeFields = model.RecipeFields;
        Assert.Equal(3, recipeFields.Count());
        AssertMandatoryFields(recipeData, recipeFields);
        RecipeFieldModel additionalNotesField = recipeFields.ElementAt(2);
        Assert.Equal("Additional notes", additionalNotesField.FieldName);
        Assert.Equal(recipeData.AdditionalNotes, additionalNotesField.FieldData);

        AssertTags(recipeData, model.RecipeTags);
    }

    [Fact]
    public void Recipe_with_title_and_invalid_character_length_throws_exception()
    {
        // Setup
        var recipeCharacterLimitProvider = Substitute.For<IRecipeModelCharacterLimitProvider>();
        recipeCharacterLimitProvider.MaximumTitleLength.Returns(10);
        recipeCharacterLimitProvider.MaximumRecipeLength.Returns(int.MaxValue);
        recipeCharacterLimitProvider.MaximumAuthorNameLength.Returns(50);
        recipeCharacterLimitProvider.MaximumFieldNameLength.Returns(50);
        recipeCharacterLimitProvider.MaximumFieldDataLength.Returns(50);
        recipeCharacterLimitProvider.MaximumRecipeTagsLength.Returns(50);

        RecipeData recipeData = CreateRecipeData(recipeCharacterLimitProvider, 1);

        var factory = new RecipeModelFactory(recipeCharacterLimitProvider);

        // Call
        Action call = () => factory.Create(recipeData);

        // Assert
        var exception = Assert.Throws<ModelCreateException>(call);
        var expectedMessage = $"RecipeTitle must be less or equal to {recipeCharacterLimitProvider.MaximumTitleLength} characters.";
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Theory]
    [InlineData(8)]
    [InlineData(0)]
    [InlineData(1)]
    public void Recipe_with_valid_title_returns_model_with_title(int characterOffset)
    {
        // Setup
        var recipeCharacterLimitProvider = Substitute.For<IRecipeModelCharacterLimitProvider>();
        recipeCharacterLimitProvider.MaximumTitleLength.Returns(10);
        recipeCharacterLimitProvider.MaximumRecipeLength.Returns(700);
        recipeCharacterLimitProvider.MaximumAuthorNameLength.Returns(50);
        recipeCharacterLimitProvider.MaximumFieldNameLength.Returns(50);
        recipeCharacterLimitProvider.MaximumFieldDataLength.Returns(50);
        recipeCharacterLimitProvider.MaximumRecipeTagsLength.Returns(500);

        RecipeData recipeData = CreateRecipeData(recipeCharacterLimitProvider, -characterOffset);

        var factory = new RecipeModelFactory(recipeCharacterLimitProvider);

        // Call
        RecipeModel model = factory.Create(recipeData);

        // Assert
        Assert.Equal(recipeData.RecipeTitle, model.Title);
    }

    [Fact]
    public void Recipe_with_data_exceeding_total_recipe_length_throws_exception()
    {
        // Setup
        var recipeCharacterLimitProvider = Substitute.For<IRecipeModelCharacterLimitProvider>();
        recipeCharacterLimitProvider.MaximumTitleLength.Returns(10);
        recipeCharacterLimitProvider.MaximumRecipeLength.Returns(1);
        recipeCharacterLimitProvider.MaximumAuthorNameLength.Returns(10);
        recipeCharacterLimitProvider.MaximumFieldNameLength.Returns(30);
        recipeCharacterLimitProvider.MaximumFieldDataLength.Returns(20);
        recipeCharacterLimitProvider.MaximumRecipeTagsLength.Returns(50);

        RecipeData recipeData = CreateRecipeData(recipeCharacterLimitProvider);

        var factory = new RecipeModelFactory(recipeCharacterLimitProvider);

        // Precondition
        int totalCharacterLength = recipeCharacterLimitProvider.MaximumAuthorNameLength +
                                   recipeCharacterLimitProvider.MaximumTitleLength +
                                   recipeCharacterLimitProvider.MaximumFieldDataLength +
                                   recipeCharacterLimitProvider.MaximumRecipeTagsLength;
        Assert.True(totalCharacterLength > recipeCharacterLimitProvider.MaximumRecipeLength);

        // Call                    
        Action call = () => factory.Create(recipeData);

        // Assert
        var exception = Assert.Throws<ModelCreateException>(call);
        var expectedMessage = $"recipeData must be less or equal to {recipeCharacterLimitProvider.MaximumRecipeLength} characters.";
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void Recipe_with_invalid_image_url_throws_exception()
    {
        // Setup
        var recipeCharacterLimitProvider = Substitute.For<IRecipeModelCharacterLimitProvider>();
        recipeCharacterLimitProvider.MaximumTitleLength.Returns(10);
        recipeCharacterLimitProvider.MaximumRecipeLength.Returns(int.MaxValue);
        recipeCharacterLimitProvider.MaximumAuthorNameLength.Returns(10);
        recipeCharacterLimitProvider.MaximumFieldNameLength.Returns(int.MaxValue);
        recipeCharacterLimitProvider.MaximumFieldDataLength.Returns(20);

        RecipeData recipeData = CreateRecipeData(recipeCharacterLimitProvider);
        recipeData.ImageUrl = "invalid image url";

        var factory = new RecipeModelFactory(recipeCharacterLimitProvider);

        // Call                    
        Action call = () => factory.Create(recipeData);

        // Assert
        Assert.Throws<ModelCreateException>(call);
    }

    private static IRecipeModelCharacterLimitProvider CreateDefaultRecipeCharacterLimitProvider()
    {
        var recipeCharacterLimitProvider = Substitute.For<IRecipeModelCharacterLimitProvider>();
        recipeCharacterLimitProvider.MaximumTitleLength.Returns(50);
        recipeCharacterLimitProvider.MaximumAuthorNameLength.Returns(50);
        recipeCharacterLimitProvider.MaximumFieldNameLength.Returns(50);
        recipeCharacterLimitProvider.MaximumFieldDataLength.Returns(50);
        recipeCharacterLimitProvider.MaximumRecipeTagsLength.Returns(500);
        recipeCharacterLimitProvider.MaximumRecipeLength.Returns(700);

        return recipeCharacterLimitProvider;
    }

    private static void AssertAuthor(AuthorData data, AuthorModel model)
    {
        Assert.Equal(data.AuthorName, model.AuthorName);
        Assert.Equal(data.AuthorImageUrl, model.AuthorImageUrl);
    }

    private static void AssertMandatoryFields(RecipeData data, IEnumerable<RecipeFieldModel> recipeFields)
    {
        RecipeFieldModel ingredientField = recipeFields.First();
        Assert.Equal("Ingredients", ingredientField.FieldName);
        Assert.Equal(data.RecipeIngredients, ingredientField.FieldData);

        RecipeFieldModel cookingStepsField = recipeFields.ElementAt(1);
        Assert.Equal("Cooking steps", cookingStepsField.FieldName);
        Assert.Equal(data.CookingSteps, cookingStepsField.FieldData);
    }

    private static void AssertTags(RecipeData data, RecipeTagsModelWrapper model)
    {
        var expectedTags = new List<string>
        {
            TagTestHelper.CategoryMapping[data.Category]
        };

        string? tagData = data.Tags;
        if (!string.IsNullOrWhiteSpace(tagData))
        {
            expectedTags.AddRange(TagTestHelper.GetParsedTags(tagData));
        }

        Assert.Equal(expectedTags, model.Tags);
    }

    private static RecipeData CreateRecipeData(IRecipeModelCharacterLimitProvider limitProvider,
                                               int nrOfTitleCharactersOffSet)
    {
        var fixture = new Fixture();
        fixture.Register(() => new string('o', limitProvider.MaximumFieldDataLength));
        AuthorData authorData = CreateAuthorData(limitProvider, fixture);

        return fixture.Build<RecipeData>()
                      .FromFactory<RecipeCategory, string>((category, field) => new RecipeData(authorData, category, new string('+', limitProvider.MaximumTitleLength + nrOfTitleCharactersOffSet),
                                                                                               field, field))
                      .Without(d => d.ImageUrl)
                      .Without(d => d.AdditionalNotes)
                      .Create();
    }

    private static RecipeData CreateRecipeData(IRecipeModelCharacterLimitProvider limitProvider)
    {
        var fixture = new Fixture();
        fixture.Register(() => new string('o', limitProvider.MaximumFieldDataLength));

        AuthorData authorData = CreateAuthorData(limitProvider, fixture);
        return fixture.Build<RecipeData>()
                      .FromFactory<RecipeCategory, string>((category, field) => new RecipeData(authorData, category, new string('+', limitProvider.MaximumTitleLength), field, field))
                      .Without(d => d.ImageUrl)
                      .Without(d => d.AdditionalNotes)
                      .Create();
    }

    private static AuthorData CreateAuthorData(IRecipeModelCharacterLimitProvider limitProvider, Fixture fixture)
    {
        return fixture.Build<AuthorData>()
                      .FromFactory(() => new AuthorData(new string('x', limitProvider.MaximumAuthorNameLength), "http://www.recipeBotImage.com"))
                      .Create();
    }
}