using eLetter25.Domain.Letters;

namespace eLetter25.Application.Letters.Ports;

/// <summary>
/// Represents a repository for managing letters.
/// </summary>
public interface ILetterRepository
{
    /// <summary>
    /// Adds a new letter to the repository.
    /// </summary>
    /// <param name="letter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddAsync(Letter letter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a letter by its unique identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Letter?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}