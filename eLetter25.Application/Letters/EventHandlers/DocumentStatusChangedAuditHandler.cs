using eLetter25.Application.Common.Events;
using eLetter25.Application.Common.Models;
using eLetter25.Application.Common.Ports;
using eLetter25.Domain.Letters.Events;
using MediatR;

namespace eLetter25.Application.Letters.EventHandlers;

/// <summary>
/// Writes an audit entry for every <see cref="LetterDocumentStatusChangedEvent"/> raised.
/// When <see cref="LetterDocumentStatusChangedEvent.PreviousStatus"/> is <c>null</c>
/// the transition represents the initial registration of the document.
/// </summary>
public sealed class DocumentStatusChangedAuditHandler(IAuditWriter auditWriter)
    : INotificationHandler<DomainEventNotification<LetterDocumentStatusChangedEvent>>
{
    public Task Handle(
        DomainEventNotification<LetterDocumentStatusChangedEvent> notification,
        CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var eventName = domainEvent.PreviousStatus is null
            ? "DocumentRegistered"
            : $"DocumentStatusChanged_{domainEvent.PreviousStatus}_{domainEvent.NewStatus}";

        var metadata = new Dictionary<string, string>
        {
            ["NewStatus"] = domainEvent.NewStatus.ToString()
        };

        if (domainEvent.PreviousStatus.HasValue)
        {
            metadata["PreviousStatus"] = domainEvent.PreviousStatus.Value.ToString();
        }

        var entry = new AuditEntry(
            eventName,
            domainEvent.DocumentId,
            nameof(LetterDocumentStatusChangedEvent),
            domainEvent.OccurredOn,
            metadata);

        return auditWriter.WriteAsync(entry, cancellationToken);
    }
}



