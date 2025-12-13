using eLetter25.Domain.Common;
using eLetter25.Domain.Letters.ValueObjects;

namespace eLetter25.Domain.Letters;

/// <summary>
/// Represents a letter with its details and correspondents.
/// </summary>
public class Letter : DomainEntity
{
    public string Subject { get; private set; } = string.Empty;

    public IReadOnlyCollection<Tag> Tags { get; private set; } = [];

    public DateTimeOffset SentDate { get; private set; }
    public DateTimeOffset CreatedDate { get; private set; } = DateTimeOffset.UtcNow;

    public Correspondent Sender { get; private set; }
    public Correspondent Recipient { get; private set; }

    public Guid? SenderReferenceId { get; private set; }
    public Guid? RecipientReferenceId { get; private set; }

    internal Letter(Correspondent sender, Correspondent recipient)
    {
        Sender = sender;
        Recipient = recipient;
    }
}