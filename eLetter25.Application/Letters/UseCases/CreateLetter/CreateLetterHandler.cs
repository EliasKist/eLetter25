using System.Security.Cryptography;
using eLetter25.Application.Common.Ports;
using eLetter25.Application.Letters.Ports;
using eLetter25.Application.Shared.DTOs;
using eLetter25.Domain.Letters;
using eLetter25.Domain.Letters.ValueObjects;
using eLetter25.Domain.Shared.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eLetter25.Application.Letters.UseCases.CreateLetter;

public sealed class CreateLetterHandler(
    ILetterRepository letterRepository,
    IUnitOfWork unitOfWork,
    IDocumentStorage documentStorage,
    ILogger<CreateLetterHandler> logger)
    : IRequestHandler<CreateLetterCommand, CreateLetterResult>
{
    public async Task<CreateLetterResult> Handle(CreateLetterCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        var sender = MapToDomain(request.Sender);
        var recipient = MapToDomain(request.Recipient);
        var initialTags = request.Tags.Select(tagName => new Tag(tagName));

        var letter = Letter.Create(sender, recipient, request.SentDate, request.Subject, initialTags);

        var (contentHash, sizeInBytes) = await ComputeHashAndSizeAsync(
            command.DocumentStream, command.DocumentSizeInBytes, cancellationToken);

        var document = letter.CreateDocument(command.DocumentFormat);
        document.StartProcessing();
        document.SetStoredMetadata(contentHash, sizeInBytes);

        await documentStorage.StoreAsync(document.Id, command.DocumentFormat, command.DocumentStream,
            cancellationToken);
        await letterRepository.AddAsync(letter, cancellationToken);

        try
        {
            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch (Exception dbException)
        {
            logger.LogError(dbException,
                "DB commit failed after storing document {DocumentId}. Attempting to delete orphaned file.",
                document.Id);

            await documentStorage.DeleteAsync(document.Id, command.DocumentFormat, CancellationToken.None);
            throw;
        }

        return new CreateLetterResult(letter.Id, document.Id);
    }

    private static async Task<(ContentHash Hash, long SizeInBytes)> ComputeHashAndSizeAsync(
        Stream stream, long reportedSize, CancellationToken cancellationToken)
    {
        if (stream.CanSeek)
        {
            stream.Seek(0, SeekOrigin.Begin);
        }

        using var sha256 = SHA256.Create();
        var hashBytes = await sha256.ComputeHashAsync(stream, cancellationToken);
        var hex = Convert.ToHexString(hashBytes).ToLowerInvariant();
        var actualSize = stream.CanSeek ? stream.Length : reportedSize;

        if (stream.CanSeek)
        {
            stream.Seek(0, SeekOrigin.Begin);
        }

        return (ContentHash.FromHex(hex), actualSize);
    }

    private static Correspondent MapToDomain(CorrespondentDto dto)
    {
        Email? email = string.IsNullOrWhiteSpace(dto.Email)
            ? null
            : new Email(dto.Email);

        PhoneNumber? phone = string.IsNullOrWhiteSpace(dto.Phone)
            ? null
            : new PhoneNumber(dto.Phone);

        return new Correspondent(
            dto.Name,
            new Address(dto.Address.Street, dto.Address.PostalCode, dto.Address.City, dto.Address.Country),
            email,
            phone);
    }
}