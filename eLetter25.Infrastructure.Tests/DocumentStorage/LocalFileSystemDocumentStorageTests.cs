using eLetter25.Application.Common.Options;
using eLetter25.Infrastructure.DocumentStorage;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Xunit;

namespace eLetter25.Infrastructure.Tests.DocumentStorage;

public sealed class LocalFileSystemDocumentStorageTests
{
    [Fact]
    public async Task StorePdfAsync_ShouldSaveFileAsDocumentIdPdf()
    {
        var tempContentRoot = Path.Combine(Path.GetTempPath(), "eletter25-tests", "content-root", Guid.NewGuid().ToString("D"));
        var relativeBasePath = Path.Combine("data", "documents");
        var documentId = Guid.NewGuid();

        var options = Options.Create(new DocumentStorageOptions { BasePath = relativeBasePath });
        var environment = new TestWebHostEnvironment(tempContentRoot);
        var sut = new LocalFileSystemDocumentStorage(options, environment);

        var pdfBytes = "%PDF-1.4\n%\u00e2\u00e3\u00cf\u00d3\n1 0 obj\n<<>>\nendobj\ntrailer\n<<>>\n%%EOF\n"u8.ToArray();
        await using var content = new MemoryStream(pdfBytes);

        var resolvedBasePath = Path.GetFullPath(Path.Combine(tempContentRoot, relativeBasePath));
        var expectedPath = Path.Combine(resolvedBasePath, $"{documentId:D}.pdf");

        try
        {
            await sut.StorePdfAsync(documentId, content, CancellationToken.None);

            File.Exists(expectedPath).Should().BeTrue();

            var storedBytes = await File.ReadAllBytesAsync(expectedPath);
            storedBytes.Should().Equal(pdfBytes);
        }
        finally
        {
            var rootToDelete = Path.Combine(Path.GetTempPath(), "eletter25-tests");
            if (Directory.Exists(rootToDelete))
            {
                Directory.Delete(rootToDelete, recursive: true);
            }
        }
    }

    [Fact]
    public async Task StorePdfAsync_ShouldThrow_WhenBasePathEscapesContentRoot()
    {
        var tempContentRoot = Path.Combine(Path.GetTempPath(), "eletter25-tests", "content-root", Guid.NewGuid().ToString("D"));
        var traversalBasePath = Path.Combine("..", "..", "sensitive-data");

        var options = Options.Create(new DocumentStorageOptions { BasePath = traversalBasePath });
        var environment = new TestWebHostEnvironment(tempContentRoot);
        var sut = new LocalFileSystemDocumentStorage(options, environment);

        var pdfBytes = "%PDF-1.4\n%%EOF\n"u8.ToArray();
        await using var content = new MemoryStream(pdfBytes);

        var action = () => sut.StorePdfAsync(Guid.NewGuid(), content, CancellationToken.None);

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*resolves outside ContentRootPath*");
    }

}

internal sealed class TestWebHostEnvironment(string contentRootPath) : IWebHostEnvironment
{
    public string ApplicationName { get; set; } = "eLetter25.Infrastructure.Tests";
    public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    public string WebRootPath { get; set; } = string.Empty;
    public string EnvironmentName { get; set; } = "Test";
    public string ContentRootPath { get; set; } = contentRootPath;
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
}
