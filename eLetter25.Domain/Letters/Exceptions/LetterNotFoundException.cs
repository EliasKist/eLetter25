using eLetter25.Domain.Common;

namespace eLetter25.Domain.Letters.Exceptions;

/// <summary>
/// Thrown when a <see cref="Letter"/> with the given identifier does not exist.
/// </summary>
public sealed class LetterNotFoundException(Guid letterId)
    : NotFoundException($"Letter '{letterId}' was not found.", "LETTER_NOT_FOUND")
{
    public Guid LetterId { get; } = letterId;
}

