namespace eLetter25.Application.Shared.DTOs;

public sealed record CorrespondentDto(
    string Name,
    AddressDto Address,
    string? Email,
    string? Phone
);