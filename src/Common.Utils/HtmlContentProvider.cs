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
using System.Net.Http;
using System.Threading.Tasks;

namespace Common.Utils;

/// <summary>
/// Class for providing html content.
/// </summary>
public class HtmlContentProvider : IHtmlContentProvider
{
    private readonly HttpClient httpClient;

    /// <summary>
    /// Creates an instance of <see cref="HtmlContentProvider"/>.
    /// </summary>
    /// <param name="httpClient">The client to retrieve the html content with.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpClient"/> is <c>null</c>.</exception>
    public HtmlContentProvider(HttpClient httpClient)
    {
        httpClient.IsNotNull(nameof(httpClient));

        this.httpClient = httpClient;
    }

    public async Task<string> GetHtmlContent(string url)
    {
        UrlValidationHelper.ValidateHttpUrl(url);

        httpClient.DefaultRequestHeaders.Add("User-Agent", "Discord RecipeBot");
        
        // TODO: Handle exceptions from the GetAsync and make sure the response is valid before the content is read
        HttpResponseMessage response = await httpClient.GetAsync(url);
        
        return await response.Content.ReadAsStringAsync();
    }
}