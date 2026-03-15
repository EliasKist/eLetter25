namespace eLetter25.Domain.Common;

/// <summary>
/// Base type for domain exceptions that represent a resource that could not be found.
/// The API layer maps this to HTTP 404.
/// </summary>
public abstract class NotFoundException(string message, string errorCode)
    : ExceptionBase(message, errorCode, details: null);

