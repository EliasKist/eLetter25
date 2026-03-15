using eLetter25.Application.Letters.Ports;
using eLetter25.Domain.Letters;
using eLetter25.Infrastructure.DomainEvents;

namespace eLetter25.Infrastructure.Persistence.Letters;

/// <summary>
/// Inserts a new <see cref="LetterDocument"/> into the database and registers its domain events
/// for dispatch on the next unit-of-work commit.
/// </summary>
public sealed class EfDocumentRepository(
    AppDbContext dbContext,
    IDomainEventCollector domainEventCollector) : IDocumentRepository
{
    public async Task AddAsync(LetterDocument document, CancellationToken cancellationToken = default)
    {
        var entity = new LetterDocumentDbEntity
        {
            Id = document.Id,
            LetterId = document.LetterId,
            DocumentFormat = document.DocumentFormat,
            Status = document.Status,
            ContentHash = document.ContentHash?.Value,
            SizeInBytes = document.SizeInBytes
        };

        await dbContext.LetterDocuments.AddAsync(entity, cancellationToken);
        domainEventCollector.CollectFrom(document);
    }
}


