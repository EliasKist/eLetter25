using eLetter25.Domain.Letters;
using eLetter25.Domain.Letters.Enums;
using MediatR;

namespace eLetter25.Application.Letters.UseCases.UploadDocument;

/// <summary>
/// Command to upload a physical document file and register it against an existing letter.
/// The resulting <see cref="LetterDocument"/> is created in <see cref="DocumentStatus.Registered"/> status.
/// </summary>
public sealed record UploadDocumentCommand(
    Guid LetterId,
    DocumentFormat DocumentFormat,
    Stream DocumentStream) : IRequest<UploadDocumentResult>;


