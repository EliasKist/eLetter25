using eLetter25.Domain.Common;

namespace eLetter25.Domain.Shared.ValueObjects;

/// <summary>
/// Represents a physical address.
/// </summary>
public readonly record struct Address
{
    public string Street { get; }
    public string PostalCode { get; }
    public string City { get; }
    public string Country { get; }

    public Address(string street, string postalCode, string city, string country)
    {
        if (string.IsNullOrWhiteSpace(street))
        {
            throw new DomainValidationException("Street cannot be null or whitespace.");
        }

        if (string.IsNullOrWhiteSpace(postalCode))
        {
            throw new DomainValidationException("Postal code cannot be null or whitespace.");
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            throw new DomainValidationException("City cannot be null or whitespace.");
        }

        if (string.IsNullOrWhiteSpace(country))
        {
            throw new DomainValidationException("Country cannot be null or whitespace.");
        }

        Street = street;
        PostalCode = postalCode;
        City = city;
        Country = country;
    }
}