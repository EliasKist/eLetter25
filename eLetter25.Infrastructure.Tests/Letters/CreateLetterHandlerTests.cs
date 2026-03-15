using System.Security.Cryptography;
using eLetter25.Application.Common.Ports;
using eLetter25.Application.Letters.Contracts;
using eLetter25.Application.Letters.Ports;
using eLetter25.Application.Letters.UseCases.CreateLetter;
using eLetter25.Application.Shared.DTOs;
using eLetter25.Domain.Letters;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace eLetter25.Infrastructure.Tests.Letters;

/// <summary>
/// Tests for the stream-handling behavior of CreateLetterHandler, specifically the
/// ComputeHashAndSizeAsync logic: correct stream reset for seekable streams, fallback to
/// reported size for non-seekable streams, and hash correctness.
/// </summary>
public sealed class CreateLetterHandlerTests
{
    [Fact]
    public async Task Handle_SeekableStream_IsResetToStartBeforeStorageUpload()
    {
        var content = "PDF content for seekable test"u8.ToArray();
        await using var stream = new MemoryStream(content);
        var storage = new CapturingDocumentStorage();

        var sut = BuildHandler(letterRepository: new FakeLetterRepository(), documentStorage: storage);
        await sut.Handle(BuildCommand(stream, reportedSize: 999), CancellationToken.None);

        // The storage must receive the stream from position 0 so the full content is written.
        storage.CapturedStreamPosition.Should().Be(0,
            "the stream must be seeked back to the beginning after hashing so storage receives all bytes");
    }

    [Fact]
    public async Task Handle_SeekableStream_UsesActualStreamLengthIgnoringReportedSize()
    {
        var content = "PDF content length test"u8.ToArray();
        await using var stream = new MemoryStream(content);
        var repo = new CapturingLetterRepository();

        var sut = BuildHandler(letterRepository: repo);
        await sut.Handle(BuildCommand(stream, reportedSize: 99_999), CancellationToken.None);

        var document = repo.CapturedLetter!.Documents.Single();
        document.SizeInBytes.Should().Be(content.Length,
            "for seekable streams the actual stream.Length must be used, not the caller-supplied estimate");
    }

    [Fact]
    public async Task Handle_NonSeekableStream_FallsBackToReportedSize()
    {
        var content = "PDF content non-seekable test"u8.ToArray();
        await using var inner = new MemoryStream(content);
        await using var nonSeekable = new NonSeekableStream(inner);
        var repo = new CapturingLetterRepository();

        var sut = BuildHandler(letterRepository: repo);
        await sut.Handle(BuildCommand(nonSeekable, reportedSize: 42), CancellationToken.None);

        var document = repo.CapturedLetter!.Documents.Single();
        document.SizeInBytes.Should().Be(42,
            "for non-seekable streams the caller-supplied reportedSize must be used as a fallback");
    }

    [Fact]
    public async Task Handle_SeekableStream_ComputesCorrectSha256Hash()
    {
        var content = "deterministic content for hash verification"u8.ToArray();
        await using var stream = new MemoryStream(content);
        var repo = new CapturingLetterRepository();

        var sut = BuildHandler(letterRepository: repo);
        await sut.Handle(BuildCommand(stream, reportedSize: content.Length), CancellationToken.None);

        var expectedHash = Convert.ToHexString(SHA256.HashData(content)).ToLowerInvariant();
        var document = repo.CapturedLetter!.Documents.Single();
        document.ContentHash.Should().NotBeNull();
        document.ContentHash!.Value.Value.Should().Be(expectedHash);
    }

    // ── Builder helpers ───────────────────────────────────────────────────────

    private static CreateLetterHandler BuildHandler(
        ILetterRepository? letterRepository = null,
        IDocumentStorage? documentStorage = null)
    {
        return new CreateLetterHandler(
            letterRepository ?? new FakeLetterRepository(),
            new FakeUnitOfWork(),
            documentStorage ?? new FakeDocumentStorage(),
            NullLogger<CreateLetterHandler>.Instance);
    }

    private static CreateLetterCommand BuildCommand(Stream documentStream, long reportedSize)
    {
        var address = new AddressDto("Main Street 1", "12345", "Berlin", "DE");
        var correspondent = new CorrespondentDto("Test Person", address, null, null);
        var request = new CreateLetterRequest(
            Subject: "Test Subject",
            SentDate: DateTimeOffset.UtcNow,
            Sender: correspondent,
            Recipient: correspondent,
            Tags: []);

        return new CreateLetterCommand(Guid.NewGuid(), request, documentStream, DocumentFormat.Pdf, reportedSize);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class CapturingLetterRepository : ILetterRepository
    {
        public Letter? CapturedLetter { get; private set; }

        public Task AddAsync(Letter letter, CancellationToken cancellationToken = default)
        {
            CapturedLetter = letter;
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<Letter>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Letter>>([]);

        public Task<Letter?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<Letter?>(null);
    }

    private sealed class FakeLetterRepository : ILetterRepository
    {
        public Task AddAsync(Letter letter, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<IReadOnlyList<Letter>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Letter>>([]);

        public Task<Letter?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<Letter?>(null);
    }

    private sealed class CapturingDocumentStorage : IDocumentStorage
    {
        public long CapturedStreamPosition { get; private set; } = -1;

        public Task StoreAsync(Guid documentId, DocumentFormat format, Stream content,
            CancellationToken cancellationToken = default)
        {
            CapturedStreamPosition = content.CanSeek ? content.Position : -1;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid documentId, DocumentFormat format,
            CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private sealed class FakeDocumentStorage : IDocumentStorage
    {
        public Task StoreAsync(Guid documentId, DocumentFormat format, Stream content,
            CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task DeleteAsync(Guid documentId, DocumentFormat format,
            CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public Task CommitAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    /// <summary>
    /// Wraps a readable stream and overrides <see cref="CanSeek"/> to return false,
    /// simulating a non-seekable source such as a network or pipe stream.
    /// </summary>
    private sealed class NonSeekableStream(Stream inner) : Stream
    {
        public override bool CanRead  => inner.CanRead;
        public override bool CanSeek  => false;
        public override bool CanWrite => false;
        public override long Length   => throw new NotSupportedException();

        public override long Position
        {
            get => inner.Position;
            set => throw new NotSupportedException();
        }

        public override void  Flush()                                           => inner.Flush();
        public override int   Read(byte[] buffer, int offset, int count)        => inner.Read(buffer, offset, count);
        public override long  Seek(long offset, SeekOrigin origin)              => throw new NotSupportedException();
        public override void  SetLength(long value)                             => throw new NotSupportedException();
        public override void  Write(byte[] buffer, int offset, int count)       => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing) inner.Dispose();
            base.Dispose(disposing);
        }
    }
}

