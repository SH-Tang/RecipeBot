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
using Discord.Interactions;
using RecipeBot.Discord.Data;
using RecipeBot.TestUtils;
using Xunit;

namespace RecipeBot.Discord.Test.Data;

public class DiscordRecipeCategoryTest
{
    [Theory]
    [MemberData(nameof(GetRecipeCategoryDisplayValues))]
    public void Category_always_displays_expected_values(
        DiscordRecipeCategory category, string expectedDisplayValue)
    {
        // Call
        ChoiceDisplayAttribute? attribute = ReflectionHelper.GetCustomAttributeFromEnum<DiscordRecipeCategory, ChoiceDisplayAttribute>(category);

        // Assert
        Assert.NotNull(attribute);
        Assert.Equal(expectedDisplayValue, attribute!.Name);
    }

    [Theory]
    [MemberData(nameof(GetRecipeCategoryOrder))]
    public void Category_always_in_expected_order(
        DiscordRecipeCategory category, int expectedOrder)
    {
        // Call
        var order = Convert.ToInt32(category);

        // Assert
        Assert.Equal(expectedOrder, order);
    }

    public static IEnumerable<object[]> GetRecipeCategoryDisplayValues()
    {
        yield return new object[]
        {
            DiscordRecipeCategory.Meat,
            "Meat"
        };

        yield return new object[]
        {
            DiscordRecipeCategory.Fish,
            "Fish"
        };

        yield return new object[]
        {
            DiscordRecipeCategory.Vegetarian,
            "Vegetarian"
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Vegan,
            "Vegan"
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Drinks,
            "Drinks"
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Pastry,
            "Pastry"
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Dessert,
            "Dessert"
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Snack,
            "Snack"
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Other,
            "Other"
        };
    }

    public static IEnumerable<object[]> GetRecipeCategoryOrder()
    {
        yield return new object[]
        {
            DiscordRecipeCategory.Meat,
            0
        };

        yield return new object[]
        {
            DiscordRecipeCategory.Fish,
            1
        };

        yield return new object[]
        {
            DiscordRecipeCategory.Vegetarian,
            2
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Vegan,
            3
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Drinks,
            4
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Pastry,
            5
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Dessert,
            6
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Snack,
            7
        };
        yield return new object[]
        {
            DiscordRecipeCategory.Other,
            8
        };
    }
}