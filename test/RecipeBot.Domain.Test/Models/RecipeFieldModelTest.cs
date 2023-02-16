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
using RecipeBot.Domain.Models;
using RecipeBot.TestUtils;
using Xunit;

namespace RecipeBot.Domain.Test.Models;

public class RecipeFieldModelTest
{
    [Theory]
    [ClassData(typeof(NullOrWhitespacesStringValueGenerator))]
    public void Model_with_invalid_field_name_throws_exception(string invalidFieldName)
    {
        // Setup
        const string fieldData = "fieldData";

        // Call
        Action call = () => new RecipeFieldModel(invalidFieldName, fieldData);

        // Assert
        call.Should().ThrowExactly<ArgumentException>();
    }

    [Theory]
    [ClassData(typeof(NullOrWhitespacesStringValueGenerator))]
    public void Model_with_invalid_field_data_throws_exception(string invalidFieldData)
    {
        // Setup
        const string fieldName = "fieldName";

        // Call
        Action call = () => new RecipeFieldModel(fieldName, invalidFieldData);

        // Assert
        call.Should().ThrowExactly<ArgumentException>();
    }

    [Theory]
    [InlineData("field name", "field data")]
    [InlineData("field name", "field    data")]
    [InlineData("field    name", "field data")]
    [InlineData("     name", "     data")]
    [InlineData("name     ", "data     ")]
    public void Model_with_valid_data_returns_total_length_of_properties(string fieldName, string fieldData)
    {
        // Setup
        var recipeField = new RecipeFieldModel(fieldName, fieldData);

        // Call
        int totalLength = recipeField.TotalLength;

        // Assert
        int expectedLength = fieldName.Length + fieldData.Length;
        totalLength.Should().Be(expectedLength);
    }
}