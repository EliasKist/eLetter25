using eLetter25.Domain.Common;
using eLetter25.Domain.Letters.Enums;

namespace eLetter25.Domain.Letters.Events;

/// <summary>
/// Raised whenever a <see cref="LetterDocument"/> transitions to a new <see cref="DocumentStatus"/>.
/// <see cref="PreviousStatus"/> is <c>null</c> for the initial <see cref="DocumentStatus.Registered"/> transition.
/// </summary>
public sealed record LetterDocumentStatusChangedEvent(
    Guid DocumentId,
    Guid LetterId,
    DocumentStatus? PreviousStatus,
    DocumentStatus NewStatus) : DomainEventBase;