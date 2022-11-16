﻿// Copyright (C) 2022 Dennis Tang. All rights reserved.
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
using RecipeBot.Discord.Providers;
using Xunit;

namespace RecipeBot.Discord.Test.Providers;

public class DiscordCharacterLimitProviderTest
{
    [Fact]
    public void Limit_provider_always_provides_correct_character_limits()
    {
        // Call
        var limitProvider = new DiscordCharacterLimitProvider();

        // Assert
        Assert.Equal(EmbedBuilder.MaxEmbedLength, limitProvider.MaximumRecipeLength);
        Assert.Equal(EmbedBuilder.MaxTitleLength, limitProvider.MaximumTitleLength);

        Assert.Equal(EmbedAuthorBuilder.MaxAuthorNameLength, limitProvider.MaximumAuthorNameLength);

        Assert.Equal(EmbedFieldBuilder.MaxFieldNameLength, limitProvider.MaximumFieldNameLength);
        Assert.Equal(EmbedFieldBuilder.MaxFieldValueLength, limitProvider.MaximumFieldDataLength);

        Assert.Equal(EmbedFooterBuilder.MaxFooterTextLength, limitProvider.MaximumRecipeTagsLength);
    }
}