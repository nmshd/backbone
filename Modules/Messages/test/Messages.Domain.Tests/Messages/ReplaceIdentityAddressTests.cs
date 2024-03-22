using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.UnitTestTools.Data;
using Backbone.UnitTestTools.Extensions;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Messages.Domain.Tests.Messages;

public class ReplaceIdentityAddressTests
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

        var message = CreateMessage((null, new List<IdentityAddress> { recipientAddress }));

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
        var testIdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var newIdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();

        var message = CreateMessage((createdByAddress, new List<IdentityAddress> { recipient1Address, recipient2Address }));

        // Act
        message.ReplaceIdentityAddress(testIdentityAddress, newIdentityAddress);

        // Assert
        message.Recipients.First().Address.Should().Be(recipient1Address);
        message.Recipients.Second().Address.Should().Be(recipient2Address);
        message.CreatedBy.Should().Be(createdByAddress);
    }

    private static Message CreateMessage((IdentityAddress createdBy, IEnumerable<IdentityAddress> recipients) parameters)
    {
        var recipientInformation = parameters.recipients.Select(recipientIdentityAddress =>
            new RecipientInformation(recipientIdentityAddress, RelationshipId.New(), [])
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
