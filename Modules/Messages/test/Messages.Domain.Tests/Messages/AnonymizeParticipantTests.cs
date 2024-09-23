using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Messages.Domain.Tests.Messages;

public class AnonymizeParticipantTests : AbstractTestsBase
{
    [Fact]
    public void CreatedBy_gets_updated()
    {
        // Arrange
        var senderAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var anonymizedAddress = TestDataGenerator.CreateRandomIdentityAddress();

        var message = CreateMessage((senderAddress, new List<IdentityAddress> { TestDataGenerator.CreateRandomIdentityAddress() }));

        // Act
        message.AnonymizeParticipant(senderAddress, anonymizedAddress);

        // Assert
        message.CreatedBy.Should().Be(anonymizedAddress);
    }

    [Fact]
    public void Recipient_gets_updated()
    {
        // Arrange
        var recipientAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var anonymizedAddress = TestDataGenerator.CreateRandomIdentityAddress();

        var message = CreateMessage((TestDataGenerator.CreateRandomIdentityAddress(), new List<IdentityAddress> { recipientAddress }));

        // Act
        message.AnonymizeParticipant(recipientAddress, anonymizedAddress);

        // Assert
        message.Recipients.Single().Address.Should().Be(anonymizedAddress);
    }

    [Fact]
    public void Message_without_identity_to_be_replaced_stays_unaffected()
    {
        // Arrange
        var recipientAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var senderAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var addressToAnonymize = TestDataGenerator.CreateRandomIdentityAddress();
        var anonymizedAddress = TestDataGenerator.CreateRandomIdentityAddress();

        var message = CreateMessage((senderAddress, new List<IdentityAddress> { recipientAddress }));

        // Act
        message.AnonymizeParticipant(addressToAnonymize, anonymizedAddress);

        // Assert
        message.Recipients.First().Address.Should().Be(recipientAddress);
        message.CreatedBy.Should().Be(senderAddress);
    }

    [Fact]
    public void Does_not_raise_a_MessageOrphanedDomainEvent_when_sender_is_not_anonymized()
    {
        // Arrange
        var senderAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var recipientAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var anonymizedAddress = TestDataGenerator.CreateRandomIdentityAddress();

        var message = CreateMessage((senderAddress, new List<IdentityAddress> { recipientAddress }));

        // Act
        message.AnonymizeParticipant(recipientAddress, anonymizedAddress);

        // Assert
        message.Should().NotHaveADomainEvent<MessageOrphanedDomainEvent>();
    }

    [Fact]
    public void Does_not_raise_a_MessageOrphanedDomainEvent_when_at_least_one_recipient_is_not_anonymized()
    {
        // Arrange
        var senderAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var recipient1Address = TestDataGenerator.CreateRandomIdentityAddress();
        var recipient2Address = TestDataGenerator.CreateRandomIdentityAddress();
        var anonymizedAddress = TestDataGenerator.CreateRandomIdentityAddress();

        var message = CreateMessage((senderAddress, new List<IdentityAddress> { recipient1Address, recipient2Address }));

        // Act
        message.AnonymizeParticipant(senderAddress, anonymizedAddress);
        message.AnonymizeParticipant(recipient1Address, anonymizedAddress);

        // Assert
        message.Should().NotHaveADomainEvent<MessageOrphanedDomainEvent>();
    }

    [Fact]
    public void Raises_a_MessageOrphanedDomainEvent_when_sender_and_all_recipients_are_anonymized()
    {
        // Arrange
        var senderAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var recipient1Address = TestDataGenerator.CreateRandomIdentityAddress();
        var recipient2Address = TestDataGenerator.CreateRandomIdentityAddress();
        var anonymizedAddress = TestDataGenerator.CreateRandomIdentityAddress();

        var message = CreateMessage((senderAddress, new List<IdentityAddress> { recipient1Address, recipient2Address }));

        // Act
        message.AnonymizeParticipant(senderAddress, anonymizedAddress);
        message.AnonymizeParticipant(recipient1Address, anonymizedAddress);
        message.AnonymizeParticipant(recipient2Address, anonymizedAddress);

        // Assert
        message.Should().HaveASingleDomainEvent<MessageOrphanedDomainEvent>();
    }

    private static Message CreateMessage((IdentityAddress createdBy, IEnumerable<IdentityAddress> recipients) parameters)
    {
        var recipientInformation = parameters.recipients.Select(recipientIdentityAddress =>
            new RecipientInformation(recipientIdentityAddress, [])
        ).ToList();

        var message = new Message(
            parameters.createdBy,
            TestDataGenerator.CreateRandomDeviceId(),
            [],
            [],
            recipientInformation
        );

        message.ClearDomainEvents();

        return message;
    }
}
