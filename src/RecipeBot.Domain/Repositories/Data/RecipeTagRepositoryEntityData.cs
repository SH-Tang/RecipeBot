namespace RecipeBot.Domain.Repositories.Data;

/// <summary>
/// Class containing the recipe tag entity data in the repository.
/// </summary>
public class RecipeTagRepositoryEntityData
{
    /// <summary>
    /// Creates a new instance of <see cref="RecipeTagRepositoryEntityData"/>.
    /// </summary>
    /// <param name="EntityId">The entity id of the recipe tag.</param>
    /// <param name="tag">The value of the tag.</param>
    public RecipeTagRepositoryEntityData(long EntityId, string tag)
    {
        this.EntityId = EntityId;
        Tag = tag;
    }

    /// <summary>
    /// Gets the tag entity id.
    /// </summary>
    public long EntityId { get; }

    /// <summary>
    /// Gets the value of the tag.
    /// </summary>
    public string Tag { get; }
}