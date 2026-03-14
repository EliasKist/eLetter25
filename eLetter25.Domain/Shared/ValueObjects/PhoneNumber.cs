using eLetter25.Domain.Common;

namespace eLetter25.Domain.Shared.ValueObjects;

/// <summary>
/// Represents a phone number.
/// </summary>
public readonly record struct PhoneNumber
{
    public string Value { get; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException("Phone number cannot be null or whitespace.");
        }

        Value = value;
    }
}