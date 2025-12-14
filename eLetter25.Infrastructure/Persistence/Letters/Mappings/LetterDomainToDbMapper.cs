using eLetter25.Domain.Letters;
using eLetter25.Domain.Letters.ValueObjects;
using eLetter25.Domain.Shared.ValueObjects;

namespace eLetter25.Infrastructure.Persistence.Letters.Mappings;

public sealed class LetterDomainToDbMapper : ILetterDomainToDbMapper
{
    public LetterDbEntity MapToDbEntity(Letter letter)
    {
        if (letter.Sender is null)
        {
            throw new InvalidOperationException("Letter.Sender must not be null when mapping to database entity.");
        }

        if (letter.Recipient is null)
        {
            throw new InvalidOperationException("Letter.Recipient must not be null when mapping to database entity.");
        }

        var senderAddress = letter.Sender.Address;
        var recipientAddress = letter.Recipient.Address;

        var entity = new LetterDbEntity
        {
            Id = letter.Id,
            Subject = letter.Subject,
            SentDate = letter.SentDate,
            CreatedDate = letter.CreatedDate,
            SenderReferenceId = letter.SenderReferenceId,
            RecipientReferenceId = letter.RecipientReferenceId,
            SenderName = letter.Sender.Name,
            SenderStreet = senderAddress.Street,
            SenderPostalCode = senderAddress.PostalCode,
            SenderCity = senderAddress.City,
            SenderCountry = senderAddress.Country,
            SenderEmail = letter.Sender.Email?.Value,
            SenderPhone = letter.Sender.Phone?.Value,
            RecipientName = letter.Recipient.Name,
            RecipientStreet = recipientAddress.Street,
            RecipientPostalCode = recipientAddress.PostalCode,
            RecipientCity = recipientAddress.City,
            RecipientCountry = recipientAddress.Country,
            RecipientEmail = letter.Recipient.Email?.Value,
            RecipientPhone = letter.Recipient.Phone?.Value,
            Tags = letter.Tags.Select(t => new LetterTagDbEntity
            {
                LetterId = letter.Id,
                Tag = t.Value,
            }).ToList(),
        };

        foreach (var tagEntity in entity.Tags)
        {
            tagEntity.Letter = entity;
        }

        return entity;
    }
}
