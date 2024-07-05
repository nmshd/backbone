using Backbone.Modules.Messages.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Tests.TestHelpers;
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
    public void SanitizeAfterRelationshipDeleted_anonymizes_recipient_and_sender_when_no_other_recipient()
    {
        // Arrange
        var anonymizedAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var message = TestData.CreateMessageWithOneRecipient();

        // Act
        message.SanitizeAfterRelationshipDeleted(message.CreatedBy, message.Recipients.First().Address, anonymizedAddress);

        // Assert
        message.CreatedBy.Should().Be(anonymizedAddress);
        message.Recipients.First().Address.Should().Be(anonymizedAddress);
    }

    [Fact]
    public void SanitizeAfterRelationshipDeleted_only_anonymizes_first_recipient_and_not_sender_when_other_recipients_exist()
    {
        // Arrange
        var senderAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var recipientAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var anonymizedAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var message = TestData.CreateMessageWithTwoRecipients(senderAddress, recipient1Address: recipientAddress);

        // Act
        message.SanitizeAfterRelationshipDeleted(senderAddress, recipientAddress, anonymizedAddress);

        // Assert
        message.CreatedBy.Should().Be(senderAddress);
        message.Recipients.First().Address.Should().Be(anonymizedAddress);
        message.Recipients.Second().Address.Should().NotBe(anonymizedAddress);
    }

    [Fact]
    public void SanitizeAfterRelationshipDeleted_only_anonymizes_second_recipient_and_not_sender_when_other_recipients_exist()
    {
        // Arrange
        var senderAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var recipientAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var anonymizedAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var message = TestData.CreateMessageWithTwoRecipients(senderAddress, recipient2Address: recipientAddress);

        // Act
        message.SanitizeAfterRelationshipDeleted(senderAddress, recipientAddress, anonymizedAddress);

        // Assert
        message.CreatedBy.Should().Be(senderAddress);
        message.Recipients.First().Address.Should().NotBe(anonymizedAddress);
        message.Recipients.Second().Address.Should().Be(anonymizedAddress);
    }
}
