using eLetter25.Domain.Letters;
using eLetter25.Domain.Letters.Events;
using eLetter25.Domain.Letters.ValueObjects;
using eLetter25.Domain.Shared.ValueObjects;
using FluentAssertions;
using Xunit;

namespace eLetter25.Domain.Tests.Letters;

public sealed class LetterDomainEventTests
{
    [Fact]
    public void Create_ShouldRaiseLetterCreatedEvent()
    {
        var sender = CreateCorrespondent("Sender");
        var recipient = CreateCorrespondent("Recipient");

        var letter = Letter.Create(sender, recipient, DateTimeOffset.UtcNow);

        var createdEvent = letter.DomainEvents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeOfType<LetterCreatedEvent>()
            .Which;

        createdEvent.LetterId.Should().Be(letter.Id);
        createdEvent.SentDate.Should().Be(letter.SentDate);
        createdEvent.CreatedDate.Should().Be(letter.CreatedDate);
    }

    [Fact]
    public void AddTag_ShouldRaiseEventOnceAndPreventDuplicates()
    {
        var sender = CreateCorrespondent("Sender");
        var recipient = CreateCorrespondent("Recipient");
        var letter = Letter.Create(sender, recipient, DateTimeOffset.UtcNow);
        letter.ClearDomainEvents();

        var tag = new Tag("Urgent");
        letter.AddTag(tag);
        letter.AddTag(tag);

        letter.Tags.Should().ContainSingle(t => t.Equals(tag));

        var tagAddedEvent = letter.DomainEvents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeOfType<LetterTagAddedEvent>()
            .Which;

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
