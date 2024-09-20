using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using Backbone.UnitTestTools.Extensions;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Messages.Domain.Tests.Messages;

public class ReplaceIdentityAddressTests : AbstractTestsBase
{
    [Fact]
    public void CreatedBy_gets_updated()
    {
        // Arrange
        var createdByAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var newIdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();

        var message = CreateMessage((createdByAddress, new List<IdentityAddress> { createdByAddress }));

        // Act
        message.ReplaceIdentityAddress(createdByAddress, newIdentityAddress);

        // Assert
        message.CreatedBy.Should().Be(newIdentityAddress);
    }

    [Fact]
    public void Recipient_gets_updated()
    {
        // Arrange
        var recipientAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var newAddress = TestDataGenerator.CreateRandomIdentityAddress();

        var message = CreateMessage((TestDataGenerator.CreateRandomIdentityAddress(), new List<IdentityAddress> { recipientAddress }));

        // Act
        message.ReplaceIdentityAddress(recipientAddress, newAddress);

        // Assert
        message.Recipients.Single().Address.Should().Be(newAddress);
    }

    [Fact]
    public void Message_without_identity_to_be_replaced_stays_unaffected()
    {
        // Arrange
        var recipient1Address = TestDataGenerator.CreateRandomIdentityAddress();
        var recipient2Address = TestDataGenerator.CreateRandomIdentityAddress();
        var createdByAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var addressToReplace = TestDataGenerator.CreateRandomIdentityAddress();
        var newIdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();

        var message = CreateMessage((createdByAddress, new List<IdentityAddress> { recipient1Address, recipient2Address }));

        // Act
        message.ReplaceIdentityAddress(addressToReplace, newIdentityAddress);

        // Assert
        message.Recipients.First().Address.Should().Be(recipient1Address);
        message.Recipients.Second().Address.Should().Be(recipient2Address);
        message.CreatedBy.Should().Be(createdByAddress);
    }

    [Fact]
    public void MessageOrphanedDomainEvent_not_raised_when_sender_is_not_anonymized()
    {
        // Arrange
        var identityA = TestDataGenerator.CreateRandomIdentityAddress();
        var identityB = TestDataGenerator.CreateRandomIdentityAddress();
        var identityC = TestDataGenerator.CreateRandomIdentityAddress();
        var newIdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();

        var message = CreateMessage((identityA, new List<IdentityAddress> { identityB, identityC }));

        // Act
        message.ReplaceIdentityAddress(identityB, newIdentityAddress);
        message.ReplaceIdentityAddress(identityC, newIdentityAddress);

        // Assert
        message.DomainEvents.Should().NotContain(e => e.GetType() == typeof(MessageOrphanedDomainEvent));
    }

    [Fact]
    public void MessageOrphanedDomainEvent_not_raised_when_one_recipient_is_not_anonymized()
    {
        // Arrange
        var identityA = TestDataGenerator.CreateRandomIdentityAddress();
        var identityB = TestDataGenerator.CreateRandomIdentityAddress();
        var identityC = TestDataGenerator.CreateRandomIdentityAddress();
        var newIdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();

        var message = CreateMessage((identityA, new List<IdentityAddress> { identityB, identityC }));

        // Act
        message.ReplaceIdentityAddress(identityA, newIdentityAddress);
        message.ReplaceIdentityAddress(identityB, newIdentityAddress);

        // Assert
        message.DomainEvents.Should().NotContain(e => e.GetType() == typeof(MessageOrphanedDomainEvent));
    }

    [Fact]
    public void MessageOrphanedDomainEvent_raised_when_sender_and_all_recipients_are_anonymized()
    {
        // Arrange
        var identityA = TestDataGenerator.CreateRandomIdentityAddress();
        var identityB = TestDataGenerator.CreateRandomIdentityAddress();
        var identityC = TestDataGenerator.CreateRandomIdentityAddress();
        var newIdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();

        var message = CreateMessage((identityA, new List<IdentityAddress> { identityB, identityC }));

        // Act
        message.ReplaceIdentityAddress(identityA, newIdentityAddress);
        message.ReplaceIdentityAddress(identityB, newIdentityAddress);
        message.ReplaceIdentityAddress(identityC, newIdentityAddress);

        // Assert
        message.DomainEvents.Should().ContainSingle(e => e.GetType() == typeof(MessageOrphanedDomainEvent));
    }

    private static Message CreateMessage((IdentityAddress createdBy, IEnumerable<IdentityAddress> recipients) parameters)
    {
        var recipientInformation = parameters.recipients.Select(recipientIdentityAddress =>
            new RecipientInformation(recipientIdentityAddress, [])
        ).ToList();

        return new Message(
            parameters.createdBy,
            TestDataGenerator.CreateRandomDeviceId(),
            [],
            [],
            recipientInformation
        );
    }
}
