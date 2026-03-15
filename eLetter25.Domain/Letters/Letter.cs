using eLetter25.Domain.Common;
using eLetter25.Domain.Letters.Events;
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
    public DateTimeOffset CreatedDate { get; private set; }

    public Correspondent? Sender { get; private set; }
    public Correspondent? Recipient { get; private set; }

    public Guid? SenderReferenceId { get; private set; }
    public Guid? RecipientReferenceId { get; private set; }

    private readonly List<LetterDocument> _documents = [];
    public IReadOnlyCollection<LetterDocument> Documents => _documents.AsReadOnly();

    private Letter()
    {
    } // For EF Core

    /// <summary>
    /// Creates a new <see cref="Letter"/> with all initial data in a single atomic step.
    /// Only <see cref="Events.LetterCreatedEvent"/> is raised; no intermediate change events are emitted.
    /// </summary>
    public static Letter Create(
        Correspondent sender,
        Correspondent recipient,
        DateTimeOffset sentDate,
        string subject,
        IEnumerable<Tag>? initialTags = null)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(recipient);

        if (sentDate == default)
        {
            throw new DomainValidationException("Sent date must be a valid date.");
        }

        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new DomainValidationException("Subject cannot be null or whitespace.");
        }

        var tags = initialTags?.Distinct().ToList() ?? [];
        var normalizedSubject = subject.Trim();

        var letter = new Letter
        {
            Sender = sender,
            Recipient = recipient,
            SentDate = sentDate,
            CreatedDate = DateTimeOffset.UtcNow,
            Subject = normalizedSubject,
            Tags = tags,
        };

        letter.Raise(new LetterCreatedEvent(
            letter.Id,
            letter.SentDate,
            letter.CreatedDate,
            letter.Subject,
            tags.Select(t => t.Value).ToArray()));

        return letter;
    }

    /// <summary>
    /// Reconstitutes a <see cref="Letter"/> from a persisted snapshot without raising domain events.
    /// Must only be called by the persistence mapper.
    /// </summary>
    public static Letter Reconstitute(
        Guid id,
        Correspondent sender,
        Correspondent recipient,
        DateTimeOffset sentDate,
        DateTimeOffset createdDate,
        string subject,
        IEnumerable<Tag> tags,
        Guid? senderReferenceId,
        Guid? recipientReferenceId)
    {
        return new Letter
        {
            Id = id,
            Sender = sender,
            Recipient = recipient,
            SentDate = sentDate,
            CreatedDate = createdDate,
            Subject = subject,
            Tags = tags.ToList(),
            SenderReferenceId = senderReferenceId,
            RecipientReferenceId = recipientReferenceId,
        };
    }

    public Letter SetSubject(string subject)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new DomainValidationException("Subject cannot be null or whitespace.");
        }

        var normalizedSubject = subject.Trim();
        if (string.Equals(Subject, normalizedSubject, StringComparison.Ordinal))
        {
            return this;
        }

        var previousSubject = Subject;
        Subject = normalizedSubject;
        Raise(new LetterSubjectChangedEvent(Id, previousSubject, Subject));
        return this;
    }

    public Letter SetSenderReferenceId(Guid senderReferenceId)
    {
        SenderReferenceId = senderReferenceId;
        return this;
    }

    public Letter SetRecipientReferenceId(Guid recipientReferenceId)
    {
        RecipientReferenceId = recipientReferenceId;
        return this;
    }


    public Letter AddTag(Tag tag)
    {
        var tags = Tags.ToList();
        if (tags.Any(existingTag => existingTag.Equals(tag)))
        {
            return this;
        }

        tags.Add(tag);
        Tags = tags;
        Raise(new LetterTagAddedEvent(Id, tag.Value));
        return this;
    }

    public Letter AddTags(IEnumerable<Tag> tags)
    {
        foreach (var tag in tags)
        {
            AddTag(tag);
        }

        return this;
    }


    public void ClearTags()
    {
        Tags = [];
    }

    public void RemoveTag(Tag tag)
    {
        var tags = Tags.ToList();
        tags.Remove(tag);
        Tags = tags;
    }

    public void RemoveTag(string tagName)
    {
        var tags = Tags.ToList();
        var tagToRemove = tags.FirstOrDefault(t => t.Value == tagName);

        tags.Remove(tagToRemove);
        Tags = tags;
    }

    public bool HasTag(string tagName)
    {
        return Tags.Any(t => t.Value == tagName);
    }

    /// <summary>
    /// Creates a new <see cref="LetterDocument"/> associated with this letter and registers it
    /// in the aggregate. Raises <see cref="Events.LetterDocumentStatusChangedEvent"/>.
    /// </summary>
    public LetterDocument CreateDocument(DocumentFormat documentFormat)
    {
        var document = new LetterDocument(Id, documentFormat);
        _documents.Add(document);
        return document;
    }

    /// <summary>
    /// Adds a reconstituted document from the persistence layer without raising domain events.
    /// Must only be called by the persistence mapper.
    /// </summary>
    internal void AddDocumentReconstituted(LetterDocument document)
    {
        _documents.Add(document);
    }
}