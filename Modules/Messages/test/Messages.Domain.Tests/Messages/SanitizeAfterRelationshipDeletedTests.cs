using Backbone.Modules.Messages.Domain.Tests.TestHelpers;
using Backbone.UnitTestTools.Data;
using Backbone.UnitTestTools.Extensions;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Messages.Domain.Tests.Messages;

public class SanitizeAfterRelationshipDeletedTests
{
    [Fact]
    public void SanitizeAfterRelationshipDeleted_anonymizes_recipient_and_sender_when_there_is_only_one_recipient()
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
    public void SanitizeAfterRelationshipDeleted_does_not_anonymize_sender_as_long_as_there_are_still_unanonymized_recipients()
    {
        // Arrange
        var senderAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var anonymizedAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var message = TestData.CreateMessageWithTwoRecipients(senderAddress);

        // Act
        message.SanitizeAfterRelationshipDeleted(senderAddress, message.Recipients.First().Address, anonymizedAddress);

        // Assert
        message.CreatedBy.Should().Be(senderAddress);

        message.Recipients.First().Address.Should().Be(anonymizedAddress);
    }

    [Fact]
    public void SanitizeAfterRelationshipDeleted_anonymizes_sender_when_there_are_no_more_unanonymized_recipients()
    {
        // Arrange
        var senderAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var anonymizedAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var message = TestData.CreateMessageWithTwoRecipients(senderAddress);

        message.SanitizeAfterRelationshipDeleted(senderAddress, message.Recipients.First().Address, anonymizedAddress);

        // Act
        message.SanitizeAfterRelationshipDeleted(senderAddress, message.Recipients.Second().Address, anonymizedAddress);

        // Assert
        message.CreatedBy.Should().Be(anonymizedAddress);

        message.Recipients.First().Address.Should().Be(anonymizedAddress);
        message.Recipients.Second().Address.Should().Be(anonymizedAddress);
    }
}
