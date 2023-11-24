using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.UnitTestTools.Data;
using Backbone.UnitTestTools.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Azure;
using Xunit;

namespace Backbone.Modules.Messages.Domain.Tests.Messages;

public class ReplaceIdentityAddressTests
{
    [Fact]
    public void CreatedBy_gets_updated()
    {
        // Arrange
        var createdByIdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var newIdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();

        var message = CreateMessage(createdByIdentityAddress);

        // Act
        message.ReplaceIdentityAddress(createdByIdentityAddress, newIdentityAddress);

        // Assert
        message.CreatedBy.Should().Be(newIdentityAddress);
    }

    [Fact]
    public void Recipient_gets_updated()
    {
        // Arrange
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var createdBydentityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var newIdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();

        var message = CreateMessage(createdBydentityAddress, new[] { identityAddress });

        // Act
        message.ReplaceIdentityAddress(identityAddress, newIdentityAddress);

        // Assert
        message.Recipients.Single().Address.Should().Be(newIdentityAddress);
    }

    [Fact]
    public void Message_without_identity_to_be_replaced_stays_unaffected()
    {
        // Arrange
        var recipient1IdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var recipient2IdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var createdByIdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var erroneousIdentityIdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var newIdentityAddress = TestDataGenerator.CreateRandomIdentityAddress();

        var message = CreateMessage(createdByIdentityAddress, new[] { recipient1IdentityAddress, recipient2IdentityAddress });

        // Act
        message.ReplaceIdentityAddress(erroneousIdentityIdentityAddress, newIdentityAddress);

        // Assert
        message.Recipients.First().Address.Should().Be(recipient1IdentityAddress);
        message.Recipients.Second().Address.Should().Be(recipient2IdentityAddress);
        message.CreatedBy.Should().Be(createdByIdentityAddress);
    }

    private static Message CreateMessage(IdentityAddress createdBy, IEnumerable<IdentityAddress> recipientsIdentityAddresses = null)
    {
        var recipientInformation = new List<RecipientInformation>();

        foreach (var recipientIdentityAddress in recipientsIdentityAddresses ?? Array.Empty<IdentityAddress>())
        {
            recipientInformation.Add(new RecipientInformation(recipientIdentityAddress, RelationshipId.New(), Array.Empty<byte>()));
        }

        return new Message(
            createdBy,
            TestDataGenerator.CreateRandomDeviceId(),
            null,
            Array.Empty<byte>(),
            Array.Empty<Attachment>(),
            recipientInformation
        );
    }

}

