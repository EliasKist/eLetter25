using eLetter25.Application.Common.Options;
using eLetter25.Infrastructure.DocumentStorage;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Xunit;

namespace eLetter25.Infrastructure.Tests.DocumentStorage;

public sealed class LocalFileSystemDocumentStorageTests
{
    [Fact]
    public async Task StorePdfAsync_ShouldSaveFileAsDocumentIdPdf()
    {
        var tempBasePath = Path.Combine(Path.GetTempPath(), "eletter25-tests", Guid.NewGuid().ToString("D"));
        var documentId = Guid.NewGuid();
        var options = Options.Create(new DocumentStorageOptions { BasePath = tempBasePath });
        var sut = new LocalFileSystemDocumentStorage(options);

        var pdfBytes = "%PDF-1.4\n%\u00e2\u00e3\u00cf\u00d3\n1 0 obj\n<<>>\nendobj\ntrailer\n<<>>\n%%EOF\n"u8.ToArray();
        await using var content = new MemoryStream(pdfBytes);

        try
        {
            await sut.StorePdfAsync(documentId, content, CancellationToken.None);

            var expectedPath = Path.Combine(tempBasePath, $"{documentId:D}.pdf");
            File.Exists(expectedPath).Should().BeTrue();

            var storedBytes = await File.ReadAllBytesAsync(expectedPath);
            storedBytes.Should().Equal(pdfBytes);
        }
        finally
        {
            if (Directory.Exists(tempBasePath))
            {
                Directory.Delete(tempBasePath, recursive: true);
            }
        }
    }
}

