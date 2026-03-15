using eLetter25.Infrastructure.DomainEvents;
using eLetter25.Infrastructure.Persistence;
using eLetter25.Infrastructure.Persistence.Letters;
using eLetter25.Infrastructure.Persistence.Letters.Mappings;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace eLetter25.Infrastructure.Tests.Letters;

/// <summary>
/// Tests for <see cref="EfLetterRepository.GetByOwnerAsync"/>.
/// Uses the EF Core in-memory provider to verify filtering, ordering, tag inclusion
/// and document exclusion without requiring a real database.
/// </summary>
public sealed class EfLetterRepositoryTests
{
    // ── Filtering ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByOwnerAsync_ReturnsOnlyLettersBelongingToOwner()
    {
        await using var ctx = CreateContext();
        var ownerId = Guid.NewGuid();
        var otherId = Guid.NewGuid();

        ctx.Letters.AddRange(
            BuildEntity(ownerId, "Letter A"),
            BuildEntity(ownerId, "Letter B"),
            BuildEntity(otherId, "Letter C – other owner"));
        await ctx.SaveChangesAsync();

        var result = await CreateSut(ctx).GetByOwnerAsync(ownerId);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(l => l.OwnerId == ownerId);
    }

    [Fact]
    public async Task GetByOwnerAsync_ReturnsEmptyList_WhenOwnerHasNoLetters()
    {
        await using var ctx = CreateContext();
        ctx.Letters.Add(BuildEntity(Guid.NewGuid(), "Someone else's letter"));
        await ctx.SaveChangesAsync();

        var result = await CreateSut(ctx).GetByOwnerAsync(Guid.NewGuid());

        result.Should().BeEmpty();
    }

    // ── Ordering ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByOwnerAsync_OrdersByCreatedDateDescending()
    {
        await using var ctx = CreateContext();
        var ownerId = Guid.NewGuid();
        var baseDate = DateTimeOffset.UtcNow;

        ctx.Letters.AddRange(
            BuildEntity(ownerId, "Old",    createdDate: baseDate.AddDays(-2)),
            BuildEntity(ownerId, "Newest", createdDate: baseDate),
            BuildEntity(ownerId, "Middle", createdDate: baseDate.AddDays(-1)));
        await ctx.SaveChangesAsync();

        var result = await CreateSut(ctx).GetByOwnerAsync(ownerId);

        result.Select(l => l.Subject).Should().ContainInOrder("Newest", "Middle", "Old");
    }

    // ── Navigation properties ─────────────────────────────────────────────────

    [Fact]
    public async Task GetByOwnerAsync_IncludesTags()
    {
        await using var ctx = CreateContext();
        var ownerId = Guid.NewGuid();
        var entity = BuildEntity(ownerId, "Tagged Letter");
        entity.Tags =
        [
            new LetterTagDbEntity { LetterId = entity.Id, Tag = "Invoice", Letter = entity },
            new LetterTagDbEntity { LetterId = entity.Id, Tag = "Urgent",  Letter = entity }
        ];

        ctx.Letters.Add(entity);
        await ctx.SaveChangesAsync();

        var result = await CreateSut(ctx).GetByOwnerAsync(ownerId);

        result.Should().ContainSingle();
        result[0].Tags.Select(t => t.Value).Should().BeEquivalentTo(["Invoice", "Urgent"]);
    }

    [Fact]
    public async Task GetByOwnerAsync_DoesNotLoadDocuments()
    {
        await using var ctx = CreateContext();
        var ownerId = Guid.NewGuid();
        var entity = BuildEntity(ownerId, "Letter with document");
        entity.Documents =
        [
            new LetterDocumentDbEntity
            {
                Id = Guid.NewGuid(),
                LetterId = entity.Id,
                DocumentFormat = Domain.Letters.DocumentFormat.Pdf,
                Status = Domain.Letters.Enums.DocumentStatus.Registered,
                Letter = entity
            }
        ];

        ctx.Letters.Add(entity);
        await ctx.SaveChangesAsync();

        var result = await CreateSut(ctx).GetByOwnerAsync(ownerId);

        result.Should().ContainSingle();
        result[0].Documents.Should().BeEmpty(
            "GetByOwnerAsync intentionally excludes documents to keep list queries lightweight");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static EfLetterRepository CreateSut(AppDbContext ctx) =>
        new(ctx, new LetterDomainToDbMapper(), new LetterDbToDomainMapper(), new DomainEventCollector());

    private static LetterDbEntity BuildEntity(
        Guid ownerId,
        string subject,
        DateTimeOffset? createdDate = null) => new()
    {
        Id = Guid.NewGuid(),
        OwnerId = ownerId,
        Subject = subject,
        SentDate = DateTimeOffset.UtcNow,
        CreatedDate = createdDate ?? DateTimeOffset.UtcNow,
        SenderName = "Sender",
        SenderStreet = "Main Street 1",
        SenderPostalCode = "12345",
        SenderCity = "Berlin",
        SenderCountry = "DE",
        RecipientName = "Recipient",
        RecipientStreet = "Side Street 2",
        RecipientPostalCode = "67890",
        RecipientCity = "Munich",
        RecipientCountry = "DE"
    };
}

