using eLetter25.Application.Common.Options;
using eLetter25.Domain.Letters;
using eLetter25.Infrastructure.DocumentStorage;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace eLetter25.Infrastructure.Tests.DocumentStorage;

public sealed class LocalFileSystemDocumentStorageTests
{
    [Fact]
    public async Task StoreAsync_ShouldSaveFileWithCorrectExtension()
    {
        var tempContentRoot  = TempContentRoot();
        var relativeBasePath = Path.Combine("data", "documents");
        var documentId       = Guid.NewGuid();

        var sut = BuildSut(tempContentRoot, relativeBasePath);

        var pdfBytes = "%PDF-1.4\n%%EOF\n"u8.ToArray();
        await using var content = new MemoryStream(pdfBytes);

        var expectedPath = Path.Combine(
            Path.GetFullPath(Path.Combine(tempContentRoot, relativeBasePath)),
            $"{documentId:D}.pdf");

        try
        {
            await sut.StoreAsync(documentId, DocumentFormat.Pdf, content, CancellationToken.None);

            File.Exists(expectedPath).Should().BeTrue();
            var storedBytes = await File.ReadAllBytesAsync(expectedPath);
            storedBytes.Should().Equal(pdfBytes);
        }
        finally
        {
            CleanUp(tempContentRoot);
        }
    }

    [Fact]
    public async Task StoreAsync_ShouldThrow_WhenBasePathEscapesContentRoot()
    {
        var tempContentRoot  = TempContentRoot();
        var traversalPath    = Path.Combine("..", "..", "sensitive-data");

        var sut = BuildSut(tempContentRoot, traversalPath);

        await using var content = new MemoryStream("%PDF-1.4\n%%EOF\n"u8.ToArray());

        var act = () => sut.StoreAsync(Guid.NewGuid(), DocumentFormat.Pdf, content, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*resolves outside ContentRootPath*");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveStoredFile()
    {
        var tempContentRoot  = TempContentRoot();
        var relativeBasePath = Path.Combine("data", "documents");
        var documentId       = Guid.NewGuid();

        var sut = BuildSut(tempContentRoot, relativeBasePath);

        await using var content = new MemoryStream("%PDF-1.4\n%%EOF\n"u8.ToArray());
        await sut.StoreAsync(documentId, DocumentFormat.Pdf, content, CancellationToken.None);

        try
        {
            await sut.DeleteAsync(documentId, DocumentFormat.Pdf, CancellationToken.None);

            var filePath = Path.Combine(
                Path.GetFullPath(Path.Combine(tempContentRoot, relativeBasePath)),
                $"{documentId:D}.pdf");

            File.Exists(filePath).Should().BeFalse();
        }
        finally
        {
            CleanUp(tempContentRoot);
        }
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotThrow_WhenFileDoesNotExist()
    {
        var sut = BuildSut(TempContentRoot(), Path.Combine("data", "docs"));
        var act = () => sut.DeleteAsync(Guid.NewGuid(), DocumentFormat.Pdf, CancellationToken.None);
        await act.Should().NotThrowAsync();
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static LocalFileSystemDocumentStorage BuildSut(string contentRoot, string basePath)
    {
        var options     = Options.Create(new DocumentStorageOptions { BasePath = basePath });
        var environment = new TestWebHostEnvironment(contentRoot);
        return new LocalFileSystemDocumentStorage(options, environment, NullLogger<LocalFileSystemDocumentStorage>.Instance);
    }

    private static string TempContentRoot() =>
        Path.Combine(Path.GetTempPath(), "eletter25-tests", "content-root", Guid.NewGuid().ToString("D"));

    private static void CleanUp(string contentRoot)
    {
        var root = Path.Combine(Path.GetTempPath(), "eletter25-tests");
        if (Directory.Exists(root))
        {
            Directory.Delete(root, recursive: true);
        }
    }
}

internal sealed class TestWebHostEnvironment(string contentRootPath) : IWebHostEnvironment
{
    public string ApplicationName { get; set; }        = "eLetter25.Infrastructure.Tests";
    public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    public string WebRootPath { get; set; }            = string.Empty;
    public string EnvironmentName { get; set; }        = "Test";
    public string ContentRootPath { get; set; }        = contentRootPath;
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
}
