namespace eLetter25.Domain.Common;

/// <summary>
/// Thrown when user-supplied data violates a domain invariant, indicating a client error (HTTP 400).
/// This distinguishes user-input failures from internal programming errors so that the API layer
/// can map them to actionable 400 responses without catching generic ArgumentExceptions.
/// </summary>
public sealed class DomainValidationException(string message, string? details = null, Exception? innerException = null)
    : ExceptionBase(message, ErrorCode, details, innerException)
{
    private new const string ErrorCode = "DOMAIN_VALIDATION_ERROR";
}

