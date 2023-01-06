using System.Collections.Generic;
using System.Linq;
using Discord;
using RecipeBot.Discord.Data;
using RecipeBot.Discord.Views;
using RecipeBot.Domain.Models;
using RecipeBot.Domain.TestUtils;
using Xunit;

namespace RecipeBot.TestUtils;

/// <summary>
/// Class that contains helper methods to assert instances of <see cref="RecipeModel"/>.
/// </summary>
public static class RecipeModelTestHelper
{
    /// <summary>
    /// Asserts a <see cref="RecipeModel"/> against data.
    /// </summary>
    /// <param name="user">The expected <see cref="IUser"/> data.</param>
    /// <param name="category">The expected <see cref="DiscordRecipeCategory"/>.</param>
    /// <param name="modal">The expected <see cref="RecipeModal"/>.</param>
    /// <param name="actualRecipe">The <see cref="RecipeModel"/> to assert.</param>
    public static void AssertCommonModelProperties(IUser user, DiscordRecipeCategory category,
                                                   RecipeModal modal, RecipeModel actualRecipe)
    {
        Assert.Equal(DiscordRecipeCategoryTestHelper.RecipeCategoryMapping[category], actualRecipe.RecipeCategory);

        AuthorModel actualAuthor = actualRecipe.Author;
        Assert.NotNull(actualAuthor);
        AssertAuthor(user.Username, user.GetAvatarUrl(), actualAuthor);

        Assert.Equal(modal.RecipeTitle, actualRecipe.Title);

        Assert.Equal(3, actualRecipe.RecipeFields.Count());
        AssertField("Ingredients", modal.Ingredients, actualRecipe.RecipeFields.ElementAt(0));
        AssertField("Cooking steps", modal.CookingSteps, actualRecipe.RecipeFields.ElementAt(1));
        AssertField("Additional notes", modal.Notes, actualRecipe.RecipeFields.ElementAt(2));

        AssertTags(modal.Tags, actualRecipe.RecipeTags);
    }

    private static void AssertAuthor(string expectedAuthorName, string expectedAuthorImageUrl, AuthorModel actualAuthor)
    {
        Assert.Equal(expectedAuthorName, actualAuthor.AuthorName);
        Assert.Equal(expectedAuthorImageUrl, actualAuthor.AuthorImageUrl);
    }

    private static void AssertField(string expectedName, string? expectedValue, RecipeFieldModel actualField)
    {
        Assert.Equal(expectedName, actualField.FieldName);
        Assert.Equal(expectedValue, actualField.FieldData);
    }

    private static void AssertTags(string? tags, RecipeTagsModelWrapper actualTags)
    {
        IEnumerable<string> expectedTags = TagTestHelper.GetParsedTags(tags);
        Assert.Equal(expectedTags, actualTags.Tags);
    }
}