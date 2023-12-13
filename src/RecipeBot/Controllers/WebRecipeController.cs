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
using System.Threading.Tasks;
using Common.Utils;
using Discord;
using Discord.Common.Providers;
using Discord.Common.Utils;
using RecipeBot.Discord.Controllers;
using RecipeBot.Discord.Data;
using RecipeBot.Discord.Views;
using RecipeBot.Domain.Data;
using RecipeBot.Services;

namespace RecipeBot.Controllers;

/// <summary>
/// A concrete implementation of <see cref="IWebRecipeController"/>.
/// </summary>
public class WebRecipeController : IWebRecipeController
{
    private readonly WebRecipeParsingService parsingService;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="provider">The <see cref="IHtmlContentProvider"/> to retrieve the web content with.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="provider"/> is <c>null</c>.</exception>
    public WebRecipeController(IHtmlContentProvider provider)
    {
        provider.IsNotNull(nameof(provider));

        parsingService = new WebRecipeParsingService(provider);
    }

    public async Task<ControllerResult<Embed>> ParseWebRecipeAsync(string webRecipeUrl, WebRecipeModal modal, IUser user, DiscordRecipeCategory category)
    {
        // TODO: Wrap this together with the date time it was parsed.
        ParsedWebRecipe parsedData = await parsingService.GetParsedWebRecipe(webRecipeUrl);

        Embed getEmbedData = GetEmbed(parsedData, webRecipeUrl, user, modal.AlternativeRecipeTitle!, category);
        return ControllerResult<Embed>.CreateControllerResultWithValidResult(getEmbedData);
    }

    private static Embed GetEmbed(ParsedWebRecipe data, string url, IUser user, string alternativeTitle, DiscordRecipeCategory category)
    {
        RecipeCategory convertedCategory = RecipeCategoryConverter.ConvertFrom(category);
        Color embedColor = RecipeCategoryConverter.ConvertTo(convertedCategory);

        var builder = new EmbedBuilder();
        builder.WithColor(embedColor);
        builder.WithUrl(url);
        builder.WithTitle(data.Title ?? alternativeTitle);

        if (data.SiteName == null)
        {
            UserData author = IUserHelper.Create(user);
            builder.WithAuthor(author.Username, author.UserImageUrl);
        }
        else
        {
            builder.WithAuthor(data.SiteName);
        }

        if (data.Description != null)
        {
            builder.WithDescription(data.Description);
        }

        if (data.ImageUrl != null)
        {
            builder.WithImageUrl(data.ImageUrl);
        }

        builder.WithTimestamp(DateTime.Now);

        return builder.Build();
    }
}