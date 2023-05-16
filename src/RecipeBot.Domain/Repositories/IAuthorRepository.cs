﻿using System.Threading.Tasks;
using RecipeBot.Domain.Exceptions;

namespace RecipeBot.Domain.Repositories;

/// <summary>
/// Interface for describing a repository for handling data persistence of author entries.
/// </summary>
public interface IAuthorRepository
{
    /// <summary>
    /// Deletes an author based on the author id.
    /// </summary>
    /// <param name="authorId">The id of the author to be deleted.</param>
    /// <returns>A <see cref="Task{TResult}"/>.</returns>
    /// <exception cref="RepositoryDataDeleteException">Thrown when the data could not be successfully deleted.</exception>
    Task DeleteEntityAsync(ulong authorId);
}