using eLetter25.Application.Letters.Contracts;
using eLetter25.Domain.Letters;
using MediatR;

namespace eLetter25.Application.Letters.UseCases.CreateLetter;

public sealed record CreateLetterCommand(
    Guid OwnerId,
    CreateLetterRequest Request,
    Stream DocumentStream,
    DocumentFormat DocumentFormat,
    long DocumentSizeInBytes) : IRequest<CreateLetterResult>;
