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
using Discord.Common.Utils;
using WeekendBot.Utils;

namespace WeekendBot.Services;

/// <summary>
/// Builder to create instances of <see cref="RecipeData"/>.
/// </summary>
public class RecipeDataBuilder
{
    private readonly RecipeData data;

    public RecipeDataBuilder(AuthorData authorData, string recipeTitle, string recipeIngredients, string recipeSteps)
    {
        data = new RecipeData(authorData, recipeTitle, recipeIngredients, recipeSteps);
    }

    public RecipeDataBuilder AddImage(IAttachment attachment)
    {
        attachment.IsNotNull(nameof(attachment));
        attachment.IsValidArgument(x => x.IsImage(), "Attachment must be an image.", nameof(attachment)); // TODO: refer to utils

        data.ImageUrl = attachment.Url;
        return this;
    }

    public RecipeDataBuilder AddNotes(string notes)
    {
        notes.IsNotNullOrWhiteSpaces(nameof(notes));

        data.AdditionalNotes = notes;
        return this;
    }

    public RecipeData Build()
    {
        return data;
    }
}