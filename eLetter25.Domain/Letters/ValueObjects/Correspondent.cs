using eLetter25.Domain.Shared.ValueObjects;

namespace eLetter25.Domain.Letters.ValueObjects;

/// <summary>
/// Represents a correspondent involved in the letter exchange.
/// </summary>
public sealed record Correspondent
{
    public string Name { get; set; }
    public Address Address { get; set; }
    public Email? Email { get; set; }
    public PhoneNumber? Phone { get; set; }

    public Correspondent(
        string name,
        Address address,
        Email? email = null,
        PhoneNumber? phone = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
        }

        Name = name;
        Address = address;
        Email = email;
        Phone = phone;
    }
}