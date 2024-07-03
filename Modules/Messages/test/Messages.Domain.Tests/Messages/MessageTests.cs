using Backbone.Modules.Messages.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using Backbone.UnitTestTools.Extensions;
using Backbone.UnitTestTools.FluentAssertions.Extensions;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Messages.Domain.Tests.Messages;

public class MessageTests : AbstractTestsBase
{
    [Fact]
    public void Raises_MessageCreatedDomainEvent_when_created()
    {
        // Arrange
        var sender = TestDataGenerator.CreateRandomIdentityAddress();
        var recipient = new RecipientInformation(TestDataGenerator.CreateRandomIdentityAddress(), []);

        // Act
        var message = new Message(
            sender,
            TestDataGenerator.CreateRandomDeviceId(),
            [],
            [],
            [recipient]
        );

        // Assert
        var domainEvent = message.Should().HaveASingleDomainEvent<MessageCreatedDomainEvent>();
        domainEvent.CreatedBy.Should().Be(sender);
        domainEvent.Id.Should().Be(message.Id);
        domainEvent.Recipients.Should().HaveCount(1);
        domainEvent.Recipients.First().Should().Be(recipient.Address);
    }

    [Fact]
    public void SanitizeAfterRelationshipDeleted_Anonymizes_Recipient_And_Sender_When_No_Other_Recipient()
    {
        // Arrange
        var sender = TestDataGenerator.CreateRandomIdentityAddress();
        var recipientAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var anonymizedAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var recipient = new RecipientInformation(recipientAddress, []);
        var message = new Message(sender, TestDataGenerator.CreateRandomDeviceId(), [], [], [recipient]);

        // Act
        message.SanitizeAfterRelationshipDeleted(sender, recipientAddress, anonymizedAddress);

        // Assert
        message.CreatedBy.Should().Be(anonymizedAddress);
        message.Recipients.First().Address.Should().Be(anonymizedAddress);
    }

    [Fact]
    public void SanitizeAfterRelationshipDeleted_Anonymizes_Recipient_Only_When_Other_Recipients_Exist()
    {
        // Arrange
        var sender = TestDataGenerator.CreateRandomIdentityAddress();
        var recipientAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var anonymizedAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var recipient = new RecipientInformation(recipientAddress, []);
        var message = new Message(sender, TestDataGenerator.CreateRandomDeviceId(), [], [], [recipient, new RecipientInformation(TestDataGenerator.CreateRandomIdentityAddress(), [])]);

        // Act
        message.SanitizeAfterRelationshipDeleted(sender, recipientAddress, anonymizedAddress);

        // Assert
        message.CreatedBy.Should().Be(sender);
        message.Recipients.First().Address.Should().Be(anonymizedAddress);
        message.Recipients.Second().Address.Should().NotBe(anonymizedAddress);
    }
}
