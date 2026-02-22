using eLetter25.Application.Common.Options;
using eLetter25.Application.Common.Ports;
using Microsoft.Extensions.Options;

namespace eLetter25.Infrastructure.DocumentStorage;

public sealed class LocalFileSystemDocumentStorage(IOptions<DocumentStorageOptions> options) : IDocumentStorage
{
    private readonly DocumentStorageOptions _options = options.Value;

    /// <inheritdoc />
    public async Task StorePdfAsync(Guid documentId, Stream pdfContent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pdfContent);

        if (documentId == Guid.Empty)
        {
            throw new ArgumentException("DocumentId must not be empty.", nameof(documentId));
        }

        if (string.IsNullOrWhiteSpace(_options.BasePath))
        {
            throw new InvalidOperationException("DocumentStorage:BasePath is not configured.");
        }

        Directory.CreateDirectory(_options.BasePath);

        var destinationPath = Path.Combine(_options.BasePath, $"{documentId:D}.pdf");
        var tempPath = Path.Combine(_options.BasePath, $"{documentId:D}.pdf.tmp");

        await using (var targetStream = new FileStream(
                         tempPath,
                         FileMode.Create,
                         FileAccess.Write,
                         FileShare.None,
                         bufferSize: 81920,
                         options: FileOptions.Asynchronous | FileOptions.SequentialScan))
        {
            if (pdfContent.CanSeek)
            {
                pdfContent.Seek(0, SeekOrigin.Begin);
            }

            await pdfContent.CopyToAsync(targetStream, cancellationToken);
            await targetStream.FlushAsync(cancellationToken);
        }

        File.Move(tempPath, destinationPath, overwrite: true);
    }
}