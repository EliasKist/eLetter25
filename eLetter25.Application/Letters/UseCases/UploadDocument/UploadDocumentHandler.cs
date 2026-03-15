using eLetter25.Application.Common.Ports;
using eLetter25.Application.Letters.Ports;
using eLetter25.Domain.Letters.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eLetter25.Application.Letters.UseCases.UploadDocument;

/// <summary>
/// Stores a physical document file against an existing letter and creates a
/// <see cref="Domain.Letters.LetterDocument"/> record in <see cref="Domain.Letters.Enums.DocumentStatus.Registered"/> status.
/// </summary>
/// <remarks>
/// Intentionally does not compute content hash or transition the document to Processing.
/// That step belongs to a subsequent processing stage that may be triggered by the
/// <see cref="Domain.Letters.Events.LetterDocumentStatusChangedEvent"/> raised here.
/// </remarks>
public sealed class UploadDocumentHandler(
    ILetterRepository letterRepository,
    IDocumentRepository documentRepository,
    IDocumentStorage documentStorage,
    IUnitOfWork unitOfWork,
    ILogger<UploadDocumentHandler> logger)
    : IRequestHandler<UploadDocumentCommand, UploadDocumentResult>
{
    public async Task<UploadDocumentResult> Handle(
        UploadDocumentCommand command,
        CancellationToken cancellationToken)
    {
        var letter = await letterRepository.GetByIdAsync(command.LetterId, cancellationToken)
                     ?? throw new LetterNotFoundException(command.LetterId);

        // CreateDocument transitions the document into Registered status and raises
        // LetterDocumentStatusChangedEvent(null → Registered) – the "DocumentRegistered" event.
        var document = letter.CreateDocument(command.DocumentFormat);

        // Non-seekable streams cannot be rewound, so buffer them upfront before storage.
        await using MemoryStream? bufferedStream = command.DocumentStream.CanSeek
            ? null
            : await BufferStreamAsync(command.DocumentStream, cancellationToken);

        var documentStream = (Stream?)bufferedStream ?? command.DocumentStream;

        if (documentStream.CanSeek)
        {
            documentStream.Seek(0, SeekOrigin.Begin);
        }

        await documentRepository.AddAsync(document, cancellationToken);
        await documentStorage.StoreAsync(document.Id, command.DocumentFormat, documentStream, cancellationToken);

        try
        {
            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch (Exception persistenceException)
        {
            logger.LogError(
                persistenceException,
                "Persistence failed after storing document {DocumentId} for letter {LetterId}. Attempting to delete orphaned file.",
                document.Id,
                command.LetterId);

            await documentStorage.DeleteAsync(document.Id, command.DocumentFormat, CancellationToken.None);
            throw;
        }

        return new UploadDocumentResult(command.LetterId, document.Id);
    }

    private static async Task<MemoryStream> BufferStreamAsync(Stream source, CancellationToken cancellationToken)
    {
        var buffer = new MemoryStream();
        await source.CopyToAsync(buffer, cancellationToken);
        buffer.Seek(0, SeekOrigin.Begin);
        return buffer;
    }
}

