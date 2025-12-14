using eLetter25.Domain.Letters;
using eLetter25.Domain.Letters.ValueObjects;
using eLetter25.Domain.Shared.ValueObjects;

namespace eLetter25.Infrastructure.Persistence.Letters.Mappings;

public sealed class LetterDbToDomainMapper : ILetterDbToDomainMapper
{
    public Letter MapToDomain(LetterDbEntity entity)
    {
        var senderAddress = new Address(
            entity.SenderStreet,
            entity.SenderPostalCode,
            entity.SenderCity,
            entity.SenderCountry);

        var recipientAddress = new Address(
            entity.RecipientStreet,
            entity.RecipientPostalCode,
            entity.RecipientCity,
            entity.RecipientCountry);

        Email? senderEmail = entity.SenderEmail is null ? null : new Email(entity.SenderEmail);
        PhoneNumber? senderPhone = entity.SenderPhone is null ? null : new PhoneNumber(entity.SenderPhone);

        Email? recipientEmail = entity.RecipientEmail is null ? null : new Email(entity.RecipientEmail);
        PhoneNumber? recipientPhone = entity.RecipientPhone is null ? null : new PhoneNumber(entity.RecipientPhone);

        var sender = new Correspondent(entity.SenderName, senderAddress, senderEmail, senderPhone);
        var recipient = new Correspondent(entity.RecipientName, recipientAddress, recipientEmail, recipientPhone);

        var letter = Letter.Create(sender, recipient, entity.SentDate)
            .SetSubject(entity.Subject);

        if (entity.SenderReferenceId.HasValue)
        {
            letter.SetSenderReferenceId(entity.SenderReferenceId.Value);
        }

        if (entity.RecipientReferenceId.HasValue)
        {
            letter.SetRecipientReferenceId(entity.RecipientReferenceId.Value);
        }

        letter.ClearTags();
        letter.AddTags(entity.Tags.Select(t => new Tag(t.Tag)));

        return letter;
    }
}
