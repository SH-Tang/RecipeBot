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
using Discord;
using Discord.Common.Services;
using Discord.Interactions;
using HtmlAgilityPack;

namespace RecipeBot.Discord;

/// <summary>
/// Module containing commands to interact with web recipes.
/// </summary>
public class WebRecipeInteractionModule : DiscordInteractionModuleBase
{
    private readonly IHtmlContentProvider provider;

    /// <summary>
    /// Creates a new instance of <see cref="WebRecipeInteractionModule"/>.
    /// </summary>
    /// <param name="provider">The provider to retrieve html content with.</param>
    /// <param name="logger">The logger to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
    public WebRecipeInteractionModule(IHtmlContentProvider provider, ILoggingService logger) : base(logger)
    {
        provider.IsNotNull(nameof(provider));

        this.provider = provider;
    }

    [SlashCommand("webrecipe-parse", "Parses a web recipe.")]
    public Task ParseWebRecipe([Summary("WebRecipe", "The website of the recipe to parse")] string webRecipe)
    {
        return ExecuteControllerAction(async () =>
        {
            string content = await provider.GetHtmlContent(webRecipe);
            ParsedWebRecipeDataWrapper parsedData = GetParsedWebRecipe(content);

            await RespondAsync(embed:GetEmbed(parsedData, webRecipe));
        });
    }

    private static Embed GetEmbed(ParsedWebRecipeDataWrapper data, string url)
    {
        var builder = new EmbedBuilder();
        builder.WithUrl(url);
        builder.WithTitle(data.WebRecipeData.Title);
        builder.WithDescription(data.WebRecipeData.Description);
        builder.WithImageUrl(data.WebRecipeData.ImageUrl);
        builder.WithTimestamp(data.RetrievalDate);
        
        return builder.Build();
    }

    private static ParsedWebRecipeDataWrapper GetParsedWebRecipe(string content)
    {
        ParsedWebRecipeData data =  ParseHtmlWebRecipe(content);

        // Also hold the following the data:
        // - Author (user)
        // - Tags
        // - Category (for color later)
        // - RecipeField (notes)

        return new ParsedWebRecipeDataWrapper(data);
    }

    private static ParsedWebRecipeData ParseHtmlWebRecipe(string content)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(content);

        XPathNavigator? navigator = doc.CreateNavigator();
        XPathNavigator? title = navigator!.SelectSingleNode("//meta[@property='og:title']/@content");
        XPathNavigator? description = navigator!.SelectSingleNode("//meta[@property='og:description']/@content");
        XPathNavigator? imageUrl = navigator!.SelectSingleNode("//meta[@property='og:image']/@content");

        // Use user title in case title could not be found.
        var parsedWebRecipeData = new ParsedWebRecipeData(title?.Value ?? "No title",
                                                          description?.Value ?? "No description available",
                                                          imageUrl?.Value);

        return parsedWebRecipeData;
    }

    private class ParsedWebRecipeDataWrapper
    {
        public ParsedWebRecipeDataWrapper(ParsedWebRecipeData webRecipeData)
        {
            webRecipeData.IsNotNull(nameof(webRecipeData));

            // Use author that made the request to be retrieved --> user data

            RetrievalDate = DateTime.Now;
            WebRecipeData = webRecipeData;
        }

        /// <summary>
        /// Gets the <see cref="DateTime"/> the recipe was retrieved.
        /// </summary>
        public DateTime RetrievalDate { get; }

        /// <summary>
        /// Gets the web recipe data.
        /// </summary>
        public ParsedWebRecipeData WebRecipeData { get; }
    }

    private class ParsedWebRecipeData
    {
        public ParsedWebRecipeData(string title, string description, string? imageUrl)
        {
            title.IsNotNullOrWhiteSpaces(nameof(title));
            description.IsNotNullOrWhiteSpaces(nameof(description));

            Title = title;
            Description = description;
            ImageUrl = imageUrl;
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title { get; }
        
        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the image url.
        /// </summary>
        public string? ImageUrl { get; }
    }
}