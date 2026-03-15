using eLetter25.Application.Letters.Contracts;

namespace eLetter25.Application.Letters.UseCases.GetLetters;

public sealed record GetLettersResult(IReadOnlyList<LetterSummaryDto> Letters);