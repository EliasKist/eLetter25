namespace eLetter25.Domain.Letters.ValueObjects;

/// <summary>
/// Represents a tag associated with a letter.
/// </summary>
public readonly record struct Tag
{
    public string Value { get; }

    public Tag(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
        }

        Value = value;
    }
}