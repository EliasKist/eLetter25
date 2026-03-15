using System.Runtime.ExceptionServices;
using eLetter25.Application.Common.Options;
using eLetter25.Application.Common.Ports;
using eLetter25.Domain.Letters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace eLetter25.Infrastructure.DocumentStorage;

public sealed class LocalFileSystemDocumentStorage(
    IOptions<DocumentStorageOptions> options,
    IWebHostEnvironment hostEnvironment,
    ILogger<LocalFileSystemDocumentStorage> logger) : IDocumentStorage
{
    private readonly DocumentStorageOptions _options = options.Value;

    private static readonly IReadOnlyDictionary<DocumentFormat, string> Extensions =
        new Dictionary<DocumentFormat, string>
        {
            [DocumentFormat.Pdf] = "pdf",
            [DocumentFormat.Png] = "png",
            [DocumentFormat.Jpeg] = "jpg"
        };

    /// <inheritdoc />
    public async Task StoreAsync(Guid documentId, DocumentFormat format, Stream content,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);

        if (documentId == Guid.Empty)
        {
            throw new ArgumentException("DocumentId must not be empty.", nameof(documentId));
        }

        var resolvedBasePath = ResolveAndValidateBasePath(_options.BasePath);
        Directory.CreateDirectory(resolvedBasePath);

        var extension = Extensions[format];
        var destination = Path.Combine(resolvedBasePath, $"{documentId:D}.{extension}");
        var temp = destination + ".tmp";

        await using (var target = new FileStream(
                         temp, FileMode.Create, FileAccess.Write, FileShare.None,
                         bufferSize: 81920, options: FileOptions.Asynchronous | FileOptions.SequentialScan))
        {
            if (content.CanSeek)
            {
                content.Seek(0, SeekOrigin.Begin);
            }

            await content.CopyToAsync(target, cancellationToken);
            await target.FlushAsync(cancellationToken);
        }

        try
        {
            File.Move(temp, destination, overwrite: true);
        }
        catch (Exception moveException)
        {
            try
            {
                if (File.Exists(temp))
                {
                    File.Delete(temp);
                }
            }
            catch (Exception cleanupException)
            {
                logger.LogError(
                    cleanupException,
                    "Failed to delete temporary file '{TempPath}' during cleanup after move failure. Manual cleanup may be required.",
                    temp);
            }

            ExceptionDispatchInfo.Capture(moveException).Throw();
            throw;
        }
    }

    /// <inheritdoc />
    public Task DeleteAsync(Guid documentId, DocumentFormat format, CancellationToken cancellationToken = default)
    {
        if (documentId == Guid.Empty)
        {
            return Task.CompletedTask;
        }

        try
        {
            var basePath = ResolveAndValidateBasePath(_options.BasePath);
            var extension = Extensions[format];
            var path = Path.Combine(basePath, $"{documentId:D}.{extension}");

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (IOException)
        {
            // Best-effort cleanup – caller must not rely on this succeeding
        }

        return Task.CompletedTask;
    }

    private string ResolveAndValidateBasePath(string basePath)
    {
        if (string.IsNullOrWhiteSpace(basePath))
        {
            throw new InvalidOperationException("DocumentStorage:BasePath is not configured.");
        }

        if (Path.IsPathRooted(basePath))
        {
            return basePath;
        }

        var contentRoot = Path.GetFullPath(hostEnvironment.ContentRootPath)
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