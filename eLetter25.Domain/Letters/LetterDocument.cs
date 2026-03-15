using eLetter25.Domain.Common;
using eLetter25.Domain.Letters.Enums;
using eLetter25.Domain.Letters.Events;
using eLetter25.Domain.Letters.Exceptions;
using eLetter25.Domain.Letters.ValueObjects;

namespace eLetter25.Domain.Letters;

/// <summary>
/// Represents a physical document file attached to a <see cref="Letter"/> and tracks its processing lifecycle.
/// </summary>
/// <remarks>
/// Expected lifecycle:
/// <code>
/// Registered → Processing → Archived
///                        ↘ ValidationNeeded → Processing (loop)
///                                          ↘ Archived
/// * → Failed → Registered (retry)
///            ↘ Deleted    (cleanup)
/// * → Deleted
/// </code>
/// </remarks>
public class LetterDocument : DomainEntity
{
    public Guid LetterId { get; private set; }
    public DocumentFormat DocumentFormat { get; private set; }
    public DocumentStatus Status { get; private set; }
    public ContentHash? ContentHash { get; private set; }
    public long? SizeInBytes { get; private set; }

    private static readonly HashSet<DocumentStatus> TerminalStatuses =
        [DocumentStatus.Archived, DocumentStatus.Deleted];

    // For EF Core
    private LetterDocument()
    {
    }

    internal LetterDocument(Guid letterId, DocumentFormat documentFormat)
    {
        LetterId = letterId;
        DocumentFormat = documentFormat;
        Status = DocumentStatus.Registered;
        Raise(new LetterDocumentStatusChangedEvent(Id, letterId, null, DocumentStatus.Registered));
    }

    /// <summary>
    /// Reconstitutes a <see cref="LetterDocument"/> from a persisted snapshot without raising domain events.
    /// Must only be called by the persistence mapper.
    /// </summary>
    public static LetterDocument Reconstitute(
        Guid id,
        Guid letterId,
        DocumentFormat documentFormat,
        DocumentStatus status,
        string? contentHashValue,
        long? sizeInBytes)
    {
        return new LetterDocument
        {
            Id = id,
            LetterId = letterId,
            DocumentFormat = documentFormat,
            Status = status,
            ContentHash = contentHashValue is null ? null : ValueObjects.ContentHash.FromHex(contentHashValue),
            SizeInBytes = sizeInBytes
        };
    }

    /// <summary>
    /// Records the storage hash and size of the physical file.
    /// Only permitted while the document is in <see cref="DocumentStatus.Processing"/> state,
    /// because metadata is only known once the file has been stored by the processing step.
    /// </summary>
    public void SetStoredMetadata(ContentHash contentHash, long sizeInBytes)
    {
        if (Status != DocumentStatus.Processing)
        {
            throw new InvalidDocumentStatusTransitionException(Id, Status, DocumentStatus.Processing);
        }

        if (ContentHash is not null && ContentHash != contentHash)
        {
            throw new ContentHashAlreadySetException(Id, ContentHash.Value, contentHash);
        }

        ArgumentOutOfRangeException.ThrowIfNegative(sizeInBytes);

        ContentHash = contentHash;
        SizeInBytes = sizeInBytes;
    }

    /// <summary>
    /// Transitions from <see cref="DocumentStatus.Registered"/> to <see cref="DocumentStatus.Processing"/>.
    /// </summary>
    public void StartProcessing() =>
        Transition(DocumentStatus.Processing, DocumentStatus.Registered);

    /// <summary>
    /// Transitions from <see cref="DocumentStatus.Processing"/> to <see cref="DocumentStatus.ValidationNeeded"/>.
    /// </summary>
    public void RequestValidation() =>
        Transition(DocumentStatus.ValidationNeeded, DocumentStatus.Processing);

    /// <summary>
    /// Transitions to <see cref="DocumentStatus.Archived"/> from Processing or ValidationNeeded.
    /// </summary>
    public void Archive()
    {
        if (Status is not (DocumentStatus.Processing or DocumentStatus.ValidationNeeded))
        {
            throw new InvalidDocumentStatusTransitionException(Id, Status, DocumentStatus.Archived);
        }

        Transition(DocumentStatus.Archived);
    }

    /// <summary>
    /// Transitions to <see cref="DocumentStatus.Failed"/> from any non-terminal status.
    /// Terminal statuses (<see cref="DocumentStatus.Archived"/>, <see cref="DocumentStatus.Deleted"/>)
    /// cannot transition to Failed.
    /// </summary>
    public void Fail()
    {
        if (TerminalStatuses.Contains(Status))
        {
            throw new InvalidDocumentStatusTransitionException(Id, Status, DocumentStatus.Failed);
        }

        Transition(DocumentStatus.Failed);
    }

    /// <summary>
    /// Resets a <see cref="DocumentStatus.Failed"/> document back to <see cref="DocumentStatus.Registered"/>
    /// so that processing can be attempted again.
    /// </summary>
    public void Retry() =>
        Transition(DocumentStatus.Registered, DocumentStatus.Failed);

    /// <summary>
    /// Transitions to <see cref="DocumentStatus.Deleted"/> from any non-deleted status.
    /// </summary>
    public void Delete()
    {
        if (Status == DocumentStatus.Deleted)
        {
            throw new InvalidDocumentStatusTransitionException(Id, Status, DocumentStatus.Deleted);
        }

        Transition(DocumentStatus.Deleted);
    }

    private void Transition(DocumentStatus target, DocumentStatus? requiredCurrent = null)
    {
        if (requiredCurrent.HasValue && Status != requiredCurrent.Value)
        {
            throw new InvalidDocumentStatusTransitionException(Id, Status, target);
        }

        var previous = Status;
        Status = target;
        Raise(new LetterDocumentStatusChangedEvent(Id, LetterId, previous, target));
    }
}