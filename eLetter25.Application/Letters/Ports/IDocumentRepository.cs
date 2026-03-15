using eLetter25.Domain.Letters;

namespace eLetter25.Application.Letters.Ports;

/// <summary>
/// Repository for standalone document persistence operations on the <see cref="LetterDocument"/> aggregate member.
/// Use this port when adding a document to an already-persisted letter.
/// </summary>
public interface IDocumentRepository
{
    /// <summary>
    /// Persists a new <see cref="LetterDocument"/> and registers it with the
    /// domain-event collector so its events are dispatched on the next commit.
    /// </summary>
    Task AddAsync(LetterDocument document, CancellationToken cancellationToken = default);
}

