namespace eLetter25.Domain.Shared.ValueObjects;

/// <summary>
/// Represents an email address.
/// </summary>
public readonly record struct Email
{
    public string Value { get; }

    public Email(string value)
    {
        // Todo: Implement proper email validation
        if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
        {
            throw new ArgumentException("Invalid email address.", nameof(value));
        }

        Value = value;
    }
}