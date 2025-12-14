namespace eLetter25.Application.Shared.DTOs;

public readonly record struct AddressDto(
    string Street,
    string PostalCode,
    string City,
    string Country
);