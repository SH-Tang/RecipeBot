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

using Discord;
using Discord.Interactions;
using FluentAssertions;
using RecipeBot.Discord.Views;
using RecipeBot.TestUtils;
using Xunit;

namespace RecipeBot.Discord.Test.Views;

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
        title.Should().Be("Recipe");
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

        InputLabelAttribute? tagsInputLabel = ReflectionHelper.GetCustomAttributeFromProperty<RecipeModal, InputLabelAttribute>(
            nameof(RecipeModal.Tags));
        ModalTextInputAttribute? tagsModalInput = ReflectionHelper.GetCustomAttributeFromProperty<RecipeModal, ModalTextInputAttribute>(
            nameof(RecipeModal.Tags));
        RequiredInputAttribute? tagsRequiredInput = ReflectionHelper.GetCustomAttributeFromProperty<RecipeModal, RequiredInputAttribute>(
            nameof(RecipeModal.Tags));

        // Assert
        AssertInputLabel("Title", titleInputLabel);
        AssertModalSingleLineInput("My recipe", EmbedBuilder.MaxTitleLength, titleModalInput);

        AssertInputLabel("Ingredients", ingredientsInputLabel);
        AssertModalParagraphInput("The ingredients of your recipe", ingredientsModalInput);

        AssertInputLabel("Steps", cookingStepsInputLabel);
        AssertModalParagraphInput("The cooking steps of your recipe", cookingStepsModalInput);

        AssertInputLabel("Notes", notesInputLabel);
        AssertModalParagraphInput("Additional notes for your recipe", notesModalInput);
        notesRequiredInput.Should().NotBeNull();
        notesRequiredInput!.IsRequired.Should().BeFalse();

        AssertInputLabel("Tags", tagsInputLabel);
        AssertModalSingleLineInput("Optional Tag1, Optional Tag2, Optional Tag3, etc", EmbedFooterBuilder.MaxFooterTextLength, tagsModalInput);
        tagsRequiredInput.Should().NotBeNull();
        tagsRequiredInput!.IsRequired.Should().BeFalse();
    }

    private static void AssertInputLabel(string expectedLabel, InputLabelAttribute? actual)
    {
        actual.Should().NotBeNull();
        actual!.Label.Should().Be(expectedLabel);
    }

    private static void AssertModalSingleLineInput(string expectedPlaceholder, int expectedMaxLength, ModalTextInputAttribute? actual)
    {
        actual.Should().NotBeNull();
        actual!.Style.Should().Be(TextInputStyle.Short);
        actual.MaxLength.Should().Be(expectedMaxLength);
        actual.Placeholder.Should().Be(expectedPlaceholder);
    }

    private static void AssertModalParagraphInput(string expectedPlaceholder, ModalTextInputAttribute? actual)
    {
        actual.Should().NotBeNull();
        actual!.Style.Should().Be(TextInputStyle.Paragraph);
        actual.MaxLength.Should().Be(EmbedFieldBuilder.MaxFieldValueLength);
        actual.Placeholder.Should().Be(expectedPlaceholder);
    }
}