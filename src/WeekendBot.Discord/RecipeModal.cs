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

namespace WeekendBot.Discord;

/// <summary>
/// Modal containing information about the recipes.
/// </summary>
public class RecipeModal : IModal
{
    public const string ModalId = "recipe_modal";

    /// <summary>
    /// Gets or sets the title of the recipe.
    /// </summary>
    [InputLabel("Title")]
    [ModalTextInput("title", maxLength: EmbedBuilder.MaxTitleLength, placeholder: "My recipe")]
    public string? RecipeTitle { get; set; }

    /// <summary>
    /// Gets or sets the ingredients of the recipe.
    /// </summary>
    [InputLabel("Ingredients")]
    [ModalTextInput("ingredients", TextInputStyle.Paragraph, maxLength: EmbedFieldBuilder.MaxFieldValueLength, placeholder: "The ingredients of your recipe")]
    public string? Ingredients { get; set; }

    /// <summary>
    /// Gets or sets the cooking steps of the recipe.
    /// </summary>
    [InputLabel("Steps")]
    [ModalTextInput("steps", TextInputStyle.Paragraph, maxLength: EmbedFieldBuilder.MaxFieldValueLength, placeholder: "The cooking steps of your recipe. Enumerations are added automatically for each new line.")]
    public string? CookingSteps { get; set; }

    /// <summary>
    /// Gets or sets additional notes of the recipe.
    /// </summary>
    [InputLabel("Notes")]
    [ModalTextInput("notes", TextInputStyle.Paragraph, maxLength: EmbedFieldBuilder.MaxFieldValueLength, placeholder: "Additional notes for your recipe")]
    [RequiredInput(false)]
    public string? Notes { get; set; }

    public string Title => "Recipe";
}