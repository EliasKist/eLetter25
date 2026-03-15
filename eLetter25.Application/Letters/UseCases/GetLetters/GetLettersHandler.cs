using eLetter25.Application.Letters.Contracts;
using eLetter25.Application.Letters.Ports;
using MediatR;

namespace eLetter25.Application.Letters.UseCases.GetLetters;

public sealed class GetLettersHandler(ILetterRepository letterRepository)
    : IRequestHandler<GetLettersQuery, GetLettersResult>
{
    public async Task<GetLettersResult> Handle(GetLettersQuery query, CancellationToken cancellationToken)
    {
        var letters = await letterRepository.GetByOwnerAsync(query.OwnerId, cancellationToken);

        var summaries = letters
            .Select(l => new LetterSummaryDto(
                l.Id,
                l.Subject,
                l.SentDate,
                l.CreatedDate,
                l.Sender?.Name ?? string.Empty,
                l.Recipient?.Name ?? string.Empty,
                l.Tags.Select(t => t.Value).ToList()))
            .ToList();

        return new GetLettersResult(summaries);
    }
}