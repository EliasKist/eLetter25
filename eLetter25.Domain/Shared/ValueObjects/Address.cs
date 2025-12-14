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
            throw new ArgumentException("Street cannot be null or whitespace.", nameof(street));
        }

        if (string.IsNullOrWhiteSpace(postalCode))
        {
            throw new ArgumentException("Postal code cannot be null or whitespace.", nameof(postalCode));
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("City cannot be null or whitespace.", nameof(city));
        }

        if (string.IsNullOrWhiteSpace(country))
        {
            throw new ArgumentException("Country cannot be null or whitespace.", nameof(country));
        }

        Street = street;
        PostalCode = postalCode;
        City = city;
        Country = country;
    }
}