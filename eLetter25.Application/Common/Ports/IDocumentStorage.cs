namespace eLetter25.Application.Common.Ports;

/// <summary>
/// Abstraction for document storage operations to allow different storage providers.
/// </summary>
public interface IDocumentStorage
{
    /// <summary>
    /// Stores a PDF document with the technical name <c>{documentId}.pdf</c>.
    /// </summary>
    /// <param name="documentId">The stable identifier used for the stored file name.</param>
    /// <param name="pdfContent">The PDF content stream. The implementation must not dispose it.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task StorePdfAsync(Guid documentId, Stream pdfContent, CancellationToken cancellationToken = default);
}

