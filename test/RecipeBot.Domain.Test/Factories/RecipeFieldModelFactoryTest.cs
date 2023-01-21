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
using FluentAssertions;
using NSubstitute;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Factories;
using RecipeBot.Domain.Models;
using RecipeBot.TestUtils;
using Xunit;

namespace RecipeBot.Domain.Test.Factories;

public class RecipeFieldModelFactoryTest
{
    [Fact]
    public void Creating_model_with_field_name_with_invalid_character_length_throws_exception()
    {
        // Setup
        const int maximumFieldNameLength = 10;
        const int maximumFieldDataLength = 20;

        var limitProvider = Substitute.For<IRecipeFieldModelCharacterLimitProvider>();
        limitProvider.MaximumFieldNameLength.Returns(maximumFieldNameLength);
        limitProvider.MaximumFieldDataLength.Returns(maximumFieldDataLength);

        var factory = new RecipeFieldModelFactory(limitProvider);

        var fieldName = new string('x', maximumFieldNameLength + 1);
        var fieldData = new string('+', maximumFieldDataLength);

        // Call
        Action call = () => factory.Create(fieldName, fieldData);

        // Assert
        var expectedMessage = $"fieldName must be less or equal to {maximumFieldNameLength} characters.";
        call.Should().Throw<ModelCreateException>()
            .WithMessage(expectedMessage);
    }

    [Theory]
    [ClassData(typeof(EmptyOrWhiteSpaceStringValueGenerator))]
    public void Creating_model_with_invalid_field_name_throws_exception(string invalidFieldName)
    {
        // Setup
        var limitProvider = Substitute.For<IRecipeFieldModelCharacterLimitProvider>();
        limitProvider.MaximumFieldNameLength.Returns(10);
        limitProvider.MaximumFieldDataLength.Returns(10);

        var factory = new RecipeFieldModelFactory(limitProvider);

        // Call
        Action call = () => factory.Create(invalidFieldName, "fieldData");

        // Assert
        call.Should().Throw<ModelCreateException>()
            .And.Message.Should().NotStartWith("fieldName must be less or equal to")
            .And.NotStartWith("fieldData must be less or equal to");
    }

    [Fact]
    public void Creating_model_with_field_data_with_invalid_character_length_throws_exception()
    {
        // Setup
        const int maximumFieldNameLength = 10;
        const int maximumFieldDataLength = 20;

        var limitProvider = Substitute.For<IRecipeFieldModelCharacterLimitProvider>();
        limitProvider.MaximumFieldNameLength.Returns(maximumFieldNameLength);
        limitProvider.MaximumFieldDataLength.Returns(maximumFieldDataLength);

        var factory = new RecipeFieldModelFactory(limitProvider);

        var fieldName = new string('x', maximumFieldNameLength);
        var fieldData = new string('+', maximumFieldDataLength + 1);

        // Call
        Action call = () => factory.Create(fieldName, fieldData);

        // Assert
        var expectedMessage = $"fieldData must be less or equal to {maximumFieldDataLength} characters.";
        call.Should().Throw<ModelCreateException>()
            .WithMessage(expectedMessage);
    }

    [Theory]
    [ClassData(typeof(EmptyOrWhiteSpaceStringValueGenerator))]
    public void Creating_model_with_invalid_field_data_throws_exception(string invalidFieldData)
    {
        // Setup
        var limitProvider = Substitute.For<IRecipeFieldModelCharacterLimitProvider>();
        limitProvider.MaximumFieldNameLength.Returns(int.MaxValue);
        limitProvider.MaximumFieldDataLength.Returns(int.MaxValue);

        var factory = new RecipeFieldModelFactory(limitProvider);

        // Call
        Action call = () => factory.Create("fieldName", invalidFieldData);

        // Assert
        call.Should().Throw<ModelCreateException>()
            .And.Message.Should().NotStartWith("fieldName must be less or equal to")
            .And.NotStartWith("fieldData must be less or equal to");
    }

    [Theory]
    [InlineData(8, 0)]
    [InlineData(0, 18)]
    [InlineData(1, 0)]
    [InlineData(0, 1)]
    [InlineData(0, 0)]
    public void Creating_model_with_valid_field_name_and_description_returns_model(
        int fieldNameCharacterOffset, int fieldDataCharacterOffset)
    {
        // Setup
        const int maximumFieldNameLength = 10;
        const int maximumFieldDataLength = 20;

        var limitProvider = Substitute.For<IRecipeFieldModelCharacterLimitProvider>();
        limitProvider.MaximumFieldNameLength.Returns(maximumFieldNameLength);
        limitProvider.MaximumFieldDataLength.Returns(maximumFieldDataLength);
        var factory = new RecipeFieldModelFactory(limitProvider);

        var fieldName = new string('x', maximumFieldNameLength - fieldNameCharacterOffset);
        var fieldData = new string('+', maximumFieldDataLength - fieldDataCharacterOffset);

        // Call
        RecipeFieldModel model = factory.Create(fieldName, fieldData);

        // Assert
        model.FieldName.Should().Be(fieldName);
        model.FieldData.Should().Be(fieldData);
    }
}