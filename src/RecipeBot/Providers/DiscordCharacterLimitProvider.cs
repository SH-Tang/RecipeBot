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
using RecipeBot.Controllers;
using RecipeBot.Domain.Factories;

namespace RecipeBot.Providers;

/// <summary>
/// Class that holds the character limits of Discord.
/// </summary>
public class DiscordCharacterLimitProvider : IRecipeModelCharacterLimitProvider, IMessageCharacterLimitProvider
{
    public int MaximumAuthorNameLength => EmbedAuthorBuilder.MaxAuthorNameLength;
    public int MaximumFieldNameLength => EmbedFieldBuilder.MaxFieldNameLength;
    public int MaximumFieldDataLength => EmbedFieldBuilder.MaxFieldValueLength;
    public int MaximumTitleLength => EmbedBuilder.MaxTitleLength;
    public int MaximumRecipeLength => EmbedBuilder.MaxEmbedLength;
    public int MaximumRecipeTagsLength => EmbedFooterBuilder.MaxFooterTextLength;
    public int MaxMessageLength => DiscordConfig.MaxMessageSize;
}