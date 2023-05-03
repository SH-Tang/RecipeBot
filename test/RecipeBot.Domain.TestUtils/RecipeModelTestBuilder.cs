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
using System.Collections.Generic;
using System.Linq;
using Common.Utils;
using RecipeBot.Domain.Data;
using RecipeBot.Domain.Models;

namespace RecipeBot.Domain.TestUtils;

/// <summary>
/// Class which creates instances of <see cref="RecipeModel"/> that can be used for testing.
/// </summary>
public class RecipeModelTestBuilder
{
    private readonly int maxFieldDataLength;
    private readonly int maxFieldNameLength;
    private readonly int maxTitleLength;

    private string? imageUrl;
    private RecipeTagsModel tags = new RecipeTagsModel(Enumerable.Empty<string>());
    private IEnumerable<RecipeFieldModel> recipeFields = Enumerable.Empty<RecipeFieldModel>();
    private RecipeCategory category = RecipeCategory.Other;

    /// <summary>
    /// Creates a new instance of <see cref="RecipeModelTestBuilder"/>.
    /// </summary>
    public RecipeModelTestBuilder() : this(new ConstructionProperties
    {
        MaxTitleLength = 256,
        MaxAuthorNameLength = 256,
        MaxFieldDataLength = 256,
        MaxFieldNameLength = 256
    }) {}

    /// <summary>
    /// Creates a new instance of <see cref="RecipeModelTestBuilder"/>.
    /// </summary>
    /// <param name="constructionProperties">The <see cref="ConstructionProperties"/> to construct the factory with.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="constructionProperties"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="constructionProperties"/> contains invalid values.</exception>
    public RecipeModelTestBuilder(ConstructionProperties constructionProperties)
    {
        constructionProperties.IsNotNull(nameof(constructionProperties));

        constructionProperties.MaxTitleLength.IsValidArgument(i => i > 0, $"{nameof(constructionProperties.MaxTitleLength)} must be larger than 0.",
                                                              nameof(constructionProperties.MaxTitleLength));
        constructionProperties.MaxAuthorNameLength.IsValidArgument(i => i > 0, $"{nameof(constructionProperties.MaxAuthorNameLength)} must be larger than 0.",
                                                                   nameof(constructionProperties.MaxAuthorNameLength));
        constructionProperties.MaxFieldNameLength.IsValidArgument(i => i > 0, $"{nameof(constructionProperties.MaxFieldNameLength)} must be larger than 0.",
                                                                  nameof(constructionProperties.MaxFieldNameLength));
        constructionProperties.MaxFieldDataLength.IsValidArgument(i => i > 0, $"{nameof(constructionProperties.MaxFieldDataLength)} must be larger than 0.",
                                                                  nameof(constructionProperties.MaxFieldDataLength));

        maxTitleLength = constructionProperties.MaxTitleLength;
        maxAuthorNameLength = constructionProperties.MaxAuthorNameLength;
        maxFieldNameLength = constructionProperties.MaxFieldNameLength;
        maxFieldDataLength = constructionProperties.MaxFieldDataLength;
    }

    /// <summary>
    /// Sets the <see cref="RecipeCategory"/> to the <see cref="RecipeModel"/>.
    /// </summary>
    /// <param name="category">The <see cref="RecipeCategory"/> to set.</param>
    /// <returns>The <see cref="RecipeModelTestBuilder"/>.</returns>
    public RecipeModelTestBuilder SetCategory(RecipeCategory category)
    {
        this.category = category;
        return this;
    }

    /// <summary>
    /// Adds an image to the <see cref="RecipeModel"/>.
    /// </summary>
    /// <returns>The <see cref="RecipeModelTestBuilder"/>.</returns>
    public RecipeModelTestBuilder AddImage()
    {
        imageUrl = "https://recipeBot.recipe.image";
        return this;
    }

    /// <summary>
    /// Adds a collection of tags to the <see cref="RecipeModel"/>.
    /// </summary>
    /// <param name="tags">The collection of tags to add.</param>
    /// <returns>The <see cref="RecipeModelTestBuilder"/>.</returns>
    public RecipeModelTestBuilder AddTags(IEnumerable<string> tags)
    {
        this.tags = new RecipeTagsModel(tags);
        return this;
    }

    /// <summary>
    /// Adds a number of recipe fields to the <see cref="RecipeModel"/>.
    /// </summary>
    /// <param name="nrOfFields">The number of fields to add.</param>
    /// <returns>The <see cref="RecipeModelTestBuilder"/>.</returns>
    public RecipeModelTestBuilder AddFields(int nrOfFields)
    {
        var random = new Random(21);
        recipeFields = Enumerable.Repeat(CreateRecipeFieldModel(random.Next()), nrOfFields);
        return this;
    }

    /// <summary>
    /// Builds the <see cref="RecipeModel"/>.
    /// </summary>
    /// <returns>A configured <see cref="RecipeModel"/>.</returns>
    public RecipeModel Build()
    {
        string title = GetStringWithRandomLength('x', maxTitleLength);
        var metaData = new RecipeModelMetaData(1337, tags, category);

        return imageUrl == null
                   ? new RecipeModel(metaData, recipeFields, title)
                   : new RecipeModel(metaData, recipeFields, title, imageUrl);
    }

    private RecipeFieldModel CreateRecipeFieldModel(int seed)
    {
        string fieldName = GetStringWithRandomLength(seed, '-', maxFieldNameLength);
        string fieldData = GetStringWithRandomLength(seed, '=', maxFieldDataLength);

        return new RecipeFieldModel(fieldName, fieldData);
    }

    private static string GetStringWithRandomLength(char character, int maximumStringLength)
    {
        return GetStringWithRandomLength(21, character, maximumStringLength);
    }

    private static string GetStringWithRandomLength(int seed, char character, int maximumStringLength)
    {
        var random = new Random(seed);
        int stringLength = maximumStringLength - random.Next(0, maximumStringLength);
        return new string(character, stringLength);
    }

    /// <summary>
    /// Class holding construction variables for <see cref="RecipeModelTestBuilder"/>.
    /// </summary>
    public class ConstructionProperties
    {
        /// <summary>
        /// Gets or sets the maximum title length.
        /// </summary>
        public int MaxTitleLength { get; init; }

        /// <summary>
        /// Gets or sets the maximum author name length.
        /// </summary>
        public int MaxAuthorNameLength { get; init; }

        /// <summary>
        /// Gets or sets the maximum field name length.
        /// </summary>
        public int MaxFieldNameLength { get; init; }

        /// <summary>
        /// Gets or sets the maximum field data length.
        /// </summary>
        public int MaxFieldDataLength { get; init; }
    }
}