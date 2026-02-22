using eLetter25.Application.Common.Options;
using eLetter25.Application.Common.Ports;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace eLetter25.Infrastructure.DocumentStorage;

public sealed class LocalFileSystemDocumentStorage(
    IOptions<DocumentStorageOptions> options,
    IWebHostEnvironment hostEnvironment) : IDocumentStorage
{
    private readonly DocumentStorageOptions _options = options.Value;
    private readonly IWebHostEnvironment _hostEnvironment = hostEnvironment;

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

        var resolvedBasePath = ResolveAndValidateBasePath(_options.BasePath);

        Directory.CreateDirectory(resolvedBasePath);

        var destinationPath = Path.Combine(resolvedBasePath, $"{documentId:D}.pdf");
        var tempPath = Path.Combine(resolvedBasePath, $"{documentId:D}.pdf.tmp");

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

        try
        {
            File.Move(tempPath, destinationPath, overwrite: true);
        }
        catch
        {
            try
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
            catch (Exception cleanupException)
            {
                throw new InvalidOperationException(
                    "Failed to move stored document to destination path and cleanup of temporary file also failed.",
                    cleanupException);
            }

            throw;
        }
    }

    private string ResolveAndValidateBasePath(string basePath)
    {
        if (Path.IsPathRooted(basePath))
        {
            return basePath;
        }

        var contentRoot = Path.GetFullPath(_hostEnvironment.ContentRootPath)
                              .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                          + Path.DirectorySeparatorChar;

        var resolved = Path.GetFullPath(Path.Combine(contentRoot, basePath));

        if (!resolved.StartsWith(contentRoot, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"DocumentStorage:BasePath resolves outside ContentRootPath. BasePath='{basePath}'.");
        }

        return resolved;
    }
}