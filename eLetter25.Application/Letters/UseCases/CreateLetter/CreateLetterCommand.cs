using eLetter25.Application.Letters.Contracts;
using MediatR;

namespace eLetter25.Application.Letters.UseCases.CreateLetter;

public sealed record CreateLetterCommand(CreateLetterRequest Request) : IRequest<CreateLetterResult>;