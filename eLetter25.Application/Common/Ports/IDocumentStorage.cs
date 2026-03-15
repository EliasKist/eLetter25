using eLetter25.Domain.Letters;

namespace eLetter25.Application.Common.Ports;

/// <summary>
/// Abstraction for document storage operations to allow different storage providers.
/// </summary>
public interface IDocumentStorage
{
    /// <summary>Stores a document under a stable content-addressed identifier.</summary>
    Task StoreAsync(Guid documentId, DocumentFormat format, Stream content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a previously stored document. No-op if the file does not exist,
    /// so it is safe to use as a compensating action after a failed DB commit.
    /// </summary>
    Task DeleteAsync(Guid documentId, DocumentFormat format, CancellationToken cancellationToken = default);
}
