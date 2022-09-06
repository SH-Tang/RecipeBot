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
using NSubstitute;
using RecipeBot.Domain.Exceptions;
using RecipeBot.Domain.Factories;
using RecipeBot.Domain.Models;
using RecipeBot.TestUtils;
using Xunit;

namespace RecipeBot.Domain.Test.Factories;

public class RecipeFieldDomainEntityFactoryTest
{
    [Fact]
    public void Creating_entity_with_field_name_with_invalid_character_length_throws_exception()
    {
        // Setup
        const int maximumFieldNameLength = 10;
        const int maximumFieldDataLength = 20;

        var limitProvider = Substitute.For<IRecipeFieldDomainEntityCharacterLimitProvider>();
        limitProvider.MaximumFieldNameLength.Returns(maximumFieldNameLength);
        limitProvider.MaximumFieldDataLength.Returns(maximumFieldDataLength);

        var factory = new RecipeFieldDomainEntityFactory(limitProvider);

        var fieldName = new string('x', maximumFieldNameLength + 1);
        var fieldData = new string('+', maximumFieldDataLength);

        // Call
        Action call = () => factory.Create(fieldName, fieldData);

        // Assert
        var exception = Assert.Throws<DomainEntityCreateException>(call);
        string expectedMessage = $"fieldName must be less or equal to {maximumFieldNameLength} characters.";
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Theory]
    [ClassData(typeof(EmptyOrWhiteSpaceStringValueGenerator))]
    public void Creating_entity_with_invalid_field_name_throws_exception(string invalidFieldName)
    {
        // Setup
        var limitProvider = Substitute.For<IRecipeFieldDomainEntityCharacterLimitProvider>();
        limitProvider.MaximumFieldNameLength.Returns(10);
        limitProvider.MaximumFieldDataLength.Returns(10);

        var factory = new RecipeFieldDomainEntityFactory(limitProvider);

        // Call
        Action call = () => factory.Create(invalidFieldName, "fieldData");

        // Assert
        var exception = Assert.Throws<DomainEntityCreateException>(call);
        string exceptionMessage = exception.Message;
        Assert.False(exceptionMessage.StartsWith("fieldName must be less or equal to"));
        Assert.False(exceptionMessage.StartsWith("fieldData must be less or equal to"));
    }

    [Fact]
    public void Creating_entity_with_field_data_with_invalid_character_length_throws_exception()
    {
        // Setup
        const int maximumFieldNameLength = 10;
        const int maximumFieldDataLength = 20;

        var limitProvider = Substitute.For<IRecipeFieldDomainEntityCharacterLimitProvider>();
        limitProvider.MaximumFieldNameLength.Returns(maximumFieldNameLength);
        limitProvider.MaximumFieldDataLength.Returns(maximumFieldDataLength);

        var factory = new RecipeFieldDomainEntityFactory(limitProvider);

        var fieldName = new string('x', maximumFieldNameLength);
        var fieldData = new string('+', maximumFieldDataLength + 1);

        // Call
        Action call = () => factory.Create(fieldName, fieldData);

        // Assert
        var exception = Assert.Throws<DomainEntityCreateException>(call);
        string expectedMessage = $"fieldData must be less or equal to {maximumFieldDataLength} characters.";
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Theory]
    [ClassData(typeof(EmptyOrWhiteSpaceStringValueGenerator))]
    public void Creating_entity_with_invalid_field_data_throws_exception(string invalidFieldData)
    {
        // Setup
        var limitProvider = Substitute.For<IRecipeFieldDomainEntityCharacterLimitProvider>();
        limitProvider.MaximumFieldNameLength.Returns(int.MaxValue);
        limitProvider.MaximumFieldDataLength.Returns(int.MaxValue);

        var factory = new RecipeFieldDomainEntityFactory(limitProvider);

        // Call
        Action call = () => factory.Create("fieldName", invalidFieldData);

        // Assert
        var exception = Assert.Throws<DomainEntityCreateException>(call);
        string exceptionMessage = exception.Message;
        Assert.False(exceptionMessage.StartsWith("fieldName must be less or equal to"));
        Assert.False(exceptionMessage.StartsWith("fieldData must be less or equal to"));
    }

    [Theory]
    [InlineData(8, 0)]
    [InlineData(0, 18)]
    [InlineData(1, 0)]
    [InlineData(0, 1)]
    [InlineData(0, 0)]
    public void Creating_entity_with_valid_field_name_and_description_returns_entity(
        int fieldNameCharacterOffset, int fieldDataCharacterOffset)
    {
        // Setup
        const int maximumFieldNameLength = 10;
        const int maximumFieldDataLength = 20;

        var limitProvider = Substitute.For<IRecipeFieldDomainEntityCharacterLimitProvider>();
        limitProvider.MaximumFieldNameLength.Returns(maximumFieldNameLength);
        limitProvider.MaximumFieldDataLength.Returns(maximumFieldDataLength);
        var factory = new RecipeFieldDomainEntityFactory(limitProvider);

        var fieldName = new string('x', maximumFieldNameLength - fieldNameCharacterOffset);
        var fieldData = new string('+', maximumFieldDataLength - fieldDataCharacterOffset);

        // Call
        RecipeFieldModel entity = factory.Create(fieldName, fieldData);

        // Assert
        Assert.Equal(fieldName, entity.FieldName);
        Assert.Equal(fieldData, entity.FieldData);
    }
}