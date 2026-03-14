using eLetter25.Domain.Common;
using eLetter25.Domain.Letters.Enums;

namespace eLetter25.Domain.Letters.Exceptions;

/// <summary>
/// Thrown when a <see cref="LetterDocument"/> transition to a target <see cref="DocumentStatus"/> is not
/// permitted from its current status.
/// </summary>
public sealed class InvalidDocumentStatusTransitionException(
    Guid documentId,
    DocumentStatus from,
    DocumentStatus to)
    : ExceptionBase(
        $"Cannot transition document '{documentId}' from '{from}' to '{to}'.",
        DefaultErrorCode,
        details: null)
{
    private const string DefaultErrorCode = "LETTER_DOCUMENT_INVALID_STATUS_TRANSITION";

    public DocumentStatus From { get; } = from;
    public DocumentStatus To { get; } = to;
}