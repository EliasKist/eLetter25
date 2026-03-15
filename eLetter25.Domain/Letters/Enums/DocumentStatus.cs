namespace eLetter25.Domain.Letters.Enums;

/// <summary>
/// Represents the lifecycle status of a <see cref="LetterDocument"/>.
/// </summary>
public enum DocumentStatus
{
    Registered,
    Processing,
    ValidationNeeded,
    Archived,
    Failed,
    Deleted
}

