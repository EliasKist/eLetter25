using MediatR;

namespace eLetter25.Application.Letters.UseCases.GetLetters;

public sealed record GetLettersQuery(Guid OwnerId) : IRequest<GetLettersResult>;


