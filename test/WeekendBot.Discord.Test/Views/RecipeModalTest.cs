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

using Discord;
using Discord.Interactions;
using WeekendBot.Discord.Views;
using WeekendBot.TestUtils;
using Xunit;

namespace WeekendBot.Discord.Test.Views;

public class RecipeModalTest
{
    [Fact]
    public void Modal_has_expected_title()
    {
        // Setup
        var modal = new RecipeModal();

        // Call
        string title = modal.Title;

        // Assert
        Assert.Equal("Recipe", title);
    }

    [Fact]
    public void Modal_has_expected_input_fields()
    {
        // Call
        InputLabelAttribute? titleInputLabel = ReflectionHelper.GetCustomAttributeFromProperty<RecipeModal, InputLabelAttribute>(
            nameof(RecipeModal.RecipeTitle));
        ModalTextInputAttribute? titleModalInput = ReflectionHelper.GetCustomAttributeFromProperty<RecipeModal, ModalTextInputAttribute>(
            nameof(RecipeModal.RecipeTitle));

        InputLabelAttribute? ingredientsInputLabel = ReflectionHelper.GetCustomAttributeFromProperty<RecipeModal, InputLabelAttribute>(
            nameof(RecipeModal.Ingredients));
        ModalTextInputAttribute? ingredientsModalInput = ReflectionHelper.GetCustomAttributeFromProperty<RecipeModal, ModalTextInputAttribute>(
            nameof(RecipeModal.Ingredients));

        InputLabelAttribute? cookingStepsInputLabel = ReflectionHelper.GetCustomAttributeFromProperty<RecipeModal, InputLabelAttribute>(
            nameof(RecipeModal.CookingSteps));
        ModalTextInputAttribute? cookingStepsModalInput = ReflectionHelper.GetCustomAttributeFromProperty<RecipeModal, ModalTextInputAttribute>(
            nameof(RecipeModal.CookingSteps));

        InputLabelAttribute? notesInputLabel = ReflectionHelper.GetCustomAttributeFromProperty<RecipeModal, InputLabelAttribute>(
            nameof(RecipeModal.Notes));
        ModalTextInputAttribute? notesModalInput = ReflectionHelper.GetCustomAttributeFromProperty<RecipeModal, ModalTextInputAttribute>(
            nameof(RecipeModal.Notes));
        RequiredInputAttribute? notesRequiredInput = ReflectionHelper.GetCustomAttributeFromProperty<RecipeModal, RequiredInputAttribute>(
            nameof(RecipeModal.Notes));

        // Assert
        AssertInputLabel("Title", titleInputLabel);
        AssertModalSingleLineInput("My recipe", titleModalInput);

        AssertInputLabel("Ingredients", ingredientsInputLabel);
        AssertModalParagraphInput("The ingredients of your recipe", ingredientsModalInput);

        AssertInputLabel("Steps", cookingStepsInputLabel);
        AssertModalParagraphInput("The cooking steps of your recipe. Enumerations are added automatically for each new line.", cookingStepsModalInput);

        AssertInputLabel("Notes", notesInputLabel);
        AssertModalParagraphInput("Additional notes for your recipe", notesModalInput);
        Assert.NotNull(notesRequiredInput);
        Assert.False(notesRequiredInput!.IsRequired);
    }

    private static void AssertInputLabel(string expectedLabel, InputLabelAttribute? actual)
    {
        Assert.NotNull(actual);
        Assert.Equal(expectedLabel, actual!.Label);
    }

    private static void AssertModalSingleLineInput(string expectedPlaceholder, ModalTextInputAttribute? actual)
    {
        Assert.NotNull(actual);
        Assert.Equal(TextInputStyle.Short, actual!.Style);
        Assert.Equal(EmbedBuilder.MaxTitleLength, actual.MaxLength);
        Assert.Equal(expectedPlaceholder, actual.Placeholder);
    }

    private static void AssertModalParagraphInput(string expectedPlaceholder, ModalTextInputAttribute? actual)
    {
        Assert.NotNull(actual);
        Assert.Equal(TextInputStyle.Paragraph, actual!.Style);
        Assert.Equal(EmbedFieldBuilder.MaxFieldValueLength, actual.MaxLength);
        Assert.Equal(expectedPlaceholder, actual.Placeholder);
    }
}