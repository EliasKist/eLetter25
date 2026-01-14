namespace eLetter25.Application.Common.Results;

public sealed record Error(string Code, string Message)
{
    public static Error Validation(string message) => new("Validation", message);
    public static Error NotFound(string message) => new("NotFound", message);
    public static Error Unauthorized(string message) => new("Unauthorized", message);
    public static Error Conflict(string message) => new("Conflict", message);
    public static Error Internal(string message) => new("Internal", message);
}