namespace eLetter25.Application.Letters.Contracts;

/// <summary>
/// A lightweight summary of a <see cref="Domain.Letters.Letter"/> used for list views.
/// </summary>
public sealed record LetterSummaryDto(
    Guid Id,
    string Subject,
    DateTimeOffset SentDate,
    DateTimeOffset CreatedDate,
    string SenderName,
    string RecipientName,
    IReadOnlyList<string> Tags);

