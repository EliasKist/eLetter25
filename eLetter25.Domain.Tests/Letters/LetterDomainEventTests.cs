using eLetter25.Domain.Letters;
using eLetter25.Domain.Letters.Events;
using eLetter25.Domain.Letters.ValueObjects;
using eLetter25.Domain.Shared.ValueObjects;
using FluentAssertions;
using Xunit;

namespace eLetter25.Domain.Tests.Letters;

public sealed class LetterDomainEventTests
{
    private static readonly Guid TestOwnerId = Guid.NewGuid();
    [Fact]
    public void Create_ShouldRaiseExactlyOneLetterCreatedEvent()
    {
        var sender = CreateCorrespondent("Sender");
        var recipient = CreateCorrespondent("Recipient");
        var tags = new[] { new Tag("Invoice"), new Tag("Urgent") };

        var letter = Letter.Create(TestOwnerId, sender, recipient, DateTimeOffset.UtcNow, "Test Subject", tags);

        var createdEvent = letter.DomainEvents
            .Should().ContainSingle("only LetterCreatedEvent must be raised during initial creation")
            .Which
            .Should().BeOfType<LetterCreatedEvent>()
            .Which;

        createdEvent.LetterId.Should().Be(letter.Id);
        createdEvent.SentDate.Should().Be(letter.SentDate);
        createdEvent.CreatedDate.Should().Be(letter.CreatedDate);
        createdEvent.Subject.Should().Be("Test Subject");
        createdEvent.InitialTags.Should().BeEquivalentTo(["Invoice", "Urgent"]);
    }

    [Fact]
    public void Create_WithoutTags_ShouldRaiseLetterCreatedEventWithEmptyTags()
    {
        var letter = Letter.Create(TestOwnerId, CreateCorrespondent("Sender"), CreateCorrespondent("Recipient"), DateTimeOffset.UtcNow, "No Tags");

        var createdEvent = letter.DomainEvents
            .Should().ContainSingle()
            .Which.Should().BeOfType<LetterCreatedEvent>().Which;

        createdEvent.InitialTags.Should().BeEmpty();
    }

    [Fact]
    public void SetSubject_AfterCreation_ShouldRaiseLetterSubjectChangedEvent()
    {
        var letter = Letter.Create(TestOwnerId, CreateCorrespondent("Sender"), CreateCorrespondent("Recipient"), DateTimeOffset.UtcNow, "Original Subject");
        letter.ClearDomainEvents();

        letter.SetSubject("Updated Subject");

        var changedEvent = letter.DomainEvents
            .Should().ContainSingle()
            .Which.Should().BeOfType<LetterSubjectChangedEvent>().Which;

        changedEvent.LetterId.Should().Be(letter.Id);
        changedEvent.PreviousSubject.Should().Be("Original Subject");
        changedEvent.CurrentSubject.Should().Be("Updated Subject");
    }

    [Fact]
    public void SetSubject_WithSameValue_ShouldNotRaiseEvent()
    {
        var letter = Letter.Create(TestOwnerId, CreateCorrespondent("Sender"), CreateCorrespondent("Recipient"), DateTimeOffset.UtcNow, "Same Subject");
        letter.ClearDomainEvents();

        letter.SetSubject("Same Subject");

        letter.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void AddTag_AfterCreation_ShouldRaiseEventOnceAndPreventDuplicates()
    {
        var letter = Letter.Create(TestOwnerId, CreateCorrespondent("Sender"), CreateCorrespondent("Recipient"), DateTimeOffset.UtcNow, "Subject");
        letter.ClearDomainEvents();

        var tag = new Tag("Urgent");
        letter.AddTag(tag);
        letter.AddTag(tag);

        letter.Tags.Should().ContainSingle(t => t.Equals(tag));

        var tagAddedEvent = letter.DomainEvents
            .Should().ContainSingle()
            .Which.Should().BeOfType<LetterTagAddedEvent>().Which;

        tagAddedEvent.LetterId.Should().Be(letter.Id);
        tagAddedEvent.Tag.Should().Be(tag.Value);
    }

    private static Correspondent CreateCorrespondent(string name)
    {
        return new Correspondent(
            name,
            new Address("Main Street", "12345", "Berlin", "DE"));
    }
}


