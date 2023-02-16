namespace RecipeBot.Domain.Repositories.Data;

/// <summary>
/// Class representing a simplified recipe tag data entry.
/// </summary>
public class RecipeTagEntryData
{
    /// <summary>
    /// Creates a new instance of <see cref="RecipeTagEntryData"/>.
    /// </summary>
    /// <param name="id">The id of the recipe tag.</param>
    /// <param name="tag">The value of the tag.</param>
    public RecipeTagEntryData(long id, string tag)
    {
        Id = id;
        Tag = tag;
    }

    /// <summary>
    /// Gets the id of the tag entry.
    /// </summary>
    public long Id { get; }

    /// <summary>
    /// Gets the value of the tag.
    /// </summary>
    public string Tag { get; }
}