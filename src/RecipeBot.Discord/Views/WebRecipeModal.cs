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

using Discord.Interactions;
using Discord;

namespace RecipeBot.Discord.Views;

/// <summary>
/// Modal containing information about web recipes.
/// </summary>
public class WebRecipeModal : IModal
{
    public const string ModalId = "web_recipe_modal";

    /// <summary>
    /// Gets or sets the alternative title of the recipe.
    /// </summary>
    [InputLabel("Title")]
    [ModalTextInput("title", maxLength: EmbedBuilder.MaxTitleLength, placeholder: "Alternative title for the web recipe in case the title cannot be parsed successfully")]
    public string? AlternativeRecipeTitle { get; set; }

    /// <summary>
    /// Gets or sets additional notes of the recipe.
    /// </summary>
    [InputLabel("Notes")]
    [ModalTextInput("notes", TextInputStyle.Paragraph, maxLength: EmbedFieldBuilder.MaxFieldValueLength, placeholder: "Additional notes for the web recipe")]
    [RequiredInput(false)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets additional tags of the recipe.
    /// </summary>
    [InputLabel("Tags")]
    [ModalTextInput("tags", maxLength: EmbedFooterBuilder.MaxFooterTextLength, placeholder: "Optional Tag1, Optional Tag2, Optional Tag3, etc")]
    [RequiredInput(false)]
    public string? Tags { get; set; }

    public string Title => "Recipe";
}