using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Messages.Domain.Tests.Messages;

public class AnonymizeParticipantTests : AbstractTestsBase
{
    [Fact]
    public void CreatedBy_gets_updated()
    {
        // Arrange
        var senderAddress = CreateRandomIdentityAddress();
        var anonymizedAddress = CreateRandomIdentityAddress();

        var message = CreateMessage((senderAddress, new List<IdentityAddress> { CreateRandomIdentityAddress() }));

        // Act
        message.AnonymizeParticipant(senderAddress, anonymizedAddress);

        // Assert
        message.CreatedBy.ShouldBe(anonymizedAddress);
    }

    [Fact]
    public void Recipient_gets_updated()
    {
        // Arrange
        var recipientAddress = CreateRandomIdentityAddress();
        var anonymizedAddress = CreateRandomIdentityAddress();

        var message = CreateMessage((CreateRandomIdentityAddress(), new List<IdentityAddress> { recipientAddress }));

        // Act
        message.AnonymizeParticipant(recipientAddress, anonymizedAddress);

        // Assert
        message.Recipients.Single().Address.ShouldBe(anonymizedAddress);
    }

    [Fact]
    public void Message_without_identity_to_be_replaced_stays_unaffected()
    {
        // Arrange
        var recipientAddress = CreateRandomIdentityAddress();
        var senderAddress = CreateRandomIdentityAddress();
        var addressToAnonymize = CreateRandomIdentityAddress();
        var anonymizedAddress = CreateRandomIdentityAddress();

        var message = CreateMessage((senderAddress, new List<IdentityAddress> { recipientAddress }));

        // Act
        message.AnonymizeParticipant(addressToAnonymize, anonymizedAddress);

        // Assert
        message.Recipients.First().Address.ShouldBe(recipientAddress);
        message.CreatedBy.ShouldBe(senderAddress);
    }

    [Fact]
    public void Does_not_raise_a_MessageOrphanedDomainEvent_when_sender_is_not_anonymized()
    {
        // Arrange
        var senderAddress = CreateRandomIdentityAddress();
        var recipientAddress = CreateRandomIdentityAddress();
        var anonymizedAddress = CreateRandomIdentityAddress();

        var message = CreateMessage((senderAddress, new List<IdentityAddress> { recipientAddress }));

        // Act
        message.AnonymizeParticipant(recipientAddress, anonymizedAddress);

        // Assert
        message.ShouldNotHaveADomainEvent<MessageOrphanedDomainEvent>();
    }

    [Fact]
    public void Does_not_raise_a_MessageOrphanedDomainEvent_when_at_least_one_recipient_is_not_anonymized()
    {
        // Arrange
        var senderAddress = CreateRandomIdentityAddress();
        var recipient1Address = CreateRandomIdentityAddress();
        var recipient2Address = CreateRandomIdentityAddress();
        var anonymizedAddress = CreateRandomIdentityAddress();

        var message = CreateMessage((senderAddress, new List<IdentityAddress> { recipient1Address, recipient2Address }));

        // Act
        message.AnonymizeParticipant(senderAddress, anonymizedAddress);
        message.AnonymizeParticipant(recipient1Address, anonymizedAddress);

        // Assert
        message.ShouldNotHaveADomainEvent<MessageOrphanedDomainEvent>();
    }

    [Fact]
    public void Raises_a_MessageOrphanedDomainEvent_when_sender_and_all_recipients_are_anonymized()
    {
        // Arrange
        var senderAddress = CreateRandomIdentityAddress();
        var recipient1Address = CreateRandomIdentityAddress();
        var recipient2Address = CreateRandomIdentityAddress();
        var anonymizedAddress = CreateRandomIdentityAddress();

        var message = CreateMessage((senderAddress, new List<IdentityAddress> { recipient1Address, recipient2Address }));

        // Act
        message.AnonymizeParticipant(senderAddress, anonymizedAddress);
        message.AnonymizeParticipant(recipient1Address, anonymizedAddress);
        message.AnonymizeParticipant(recipient2Address, anonymizedAddress);

        // Assert
        message.ShouldHaveASingleDomainEvent<MessageOrphanedDomainEvent>();
    }

    private static Message CreateMessage((IdentityAddress createdBy, IEnumerable<IdentityAddress> recipients) parameters)
    {
        var recipientInformation = parameters.recipients.Select(recipientIdentityAddress =>
            new RecipientInformation(recipientIdentityAddress, RelationshipId.New(), [])
        ).ToList();

        var message = new Message(
            parameters.createdBy,
            CreateRandomDeviceId(),
            [],
            [],
            recipientInformation
        );

        message.ClearDomainEvents();

        return message;
    }
}
