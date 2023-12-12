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
using System.Xml.XPath;
using Common.Utils;
using HtmlAgilityPack;

namespace RecipeBot.Services;

/// <summary>
/// Service responsible for parsing web recipes.
/// </summary>
internal class WebRecipeParsingService
{
    private const string xPathTitle = "//meta[@property='og:title']/@content";
    private const string xPathDescription = "//meta[@property='og:description']/@content";
    private const string xPathShortSiteName = "//meta[@property='og:site_name']/@content";
    private const string xPathImage = "//meta[@property='og:image']/@content";

    private readonly IHtmlContentProvider provider;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="provider">The <see cref="IHtmlContentProvider"/> to retrieve the web content with.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="provider"/> is <c>null</c>.</exception>
    public WebRecipeParsingService(IHtmlContentProvider provider)
    {
        provider.IsNotNull(nameof(provider));

        this.provider = provider;
    }

    public async Task<ParsedWebRecipe> GetParsedWebRecipe(string webRecipeUrl)
    {
        // Wrap this in a custom exception
        string content = await provider.GetHtmlContent(webRecipeUrl);

        var doc = new HtmlDocument();
        doc.LoadHtml(content);

        XPathNavigator navigator = doc.CreateNavigator() ?? throw new InvalidOperationException();

        return new ParsedWebRecipe
        {
            Title = navigator.SelectSingleNode(xPathTitle)?.Value,
            Description = navigator.SelectSingleNode(xPathDescription)?.Value,
            SiteName = navigator.SelectSingleNode(xPathShortSiteName)?.Value,
            ImageUrl = navigator.SelectSingleNode(xPathImage)?.Value,
        };
    }
}