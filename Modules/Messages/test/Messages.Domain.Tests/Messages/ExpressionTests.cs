using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Tests.TestHelpers;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using Backbone.UnitTestTools.Extensions;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Messages.Domain.Tests.Messages;

public class ExpressionTests : AbstractTestsBase
{
    #region WasExchangedBetween

    [Fact]
    public void WasExchangedBetween_with_true_assertion()
    {
        // Arrange
        var senderAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var recipientAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var recipient = new RecipientInformation(recipientAddress, []);
        var message = new Message(senderAddress, TestDataGenerator.CreateRandomDeviceId(), [], [], [recipient]);

        // Act
        var resultOne = message.EvaluateWasExchangedBetweenExpression(senderAddress, recipientAddress);
        var resultTwo = message.EvaluateWasExchangedBetweenExpression(recipientAddress, senderAddress);

        // Assert
        resultOne.Should().BeTrue();
        resultTwo.Should().BeTrue();
    }

    [Fact]
    public void WasExchangedBetween_with_false_assertion()
    {
        // Arrange
        var senderAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var recipientAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var recipient = new RecipientInformation(recipientAddress, []);
        var message = new Message(senderAddress, TestDataGenerator.CreateRandomDeviceId(), [], [], [recipient]);

        // Act
        var result = message.EvaluateWasExchangedBetweenExpression(senderAddress, TestDataGenerator.CreateRandomIdentityAddress());

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region HasParticipant

    private static readonly IdentityAddress AnonymizedAddress = TestDataGenerator.CreateRandomIdentityAddress();

    [Fact]
    public void HasParticipant_is_true_for_sender_when_no_relationship_is_decomposed()
    {
        // Arrange
        var message = TestData.CreateMessageWithOneRecipient();

        // Act
        var result = message.EvaluateHasParticipantExpression(message.CreatedBy);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasParticipant_is_true_for_sender_when_relationship_to_at_least_one_recipient_is_not_decomposed()
    {
        // Arrange
        var message = TestData.CreateMessageWithTwoRecipients();
        message.DecomposeFor(message.CreatedBy, message.Recipients.First().Address, AnonymizedAddress);

        // Act
        var result = message.EvaluateHasParticipantExpression(message.CreatedBy);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasParticipant_is_false_for_sender_when_relationship_to_all_recipients_are_decomposed()
    {
        // Arrange
        var message = TestData.CreateMessageWithTwoRecipients();
        message.DecomposeFor(message.CreatedBy, message.Recipients.First().Address, AnonymizedAddress);
        message.DecomposeFor(message.CreatedBy, message.Recipients.Second().Address, AnonymizedAddress);

        // Act
        var result = message.EvaluateHasParticipantExpression(message.CreatedBy);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HasParticipant_is_true_for_recipient_when_relationship_to_sender_is_not_decomposed()
    {
        // Arrange
        var message = TestData.CreateMessageWithTwoRecipients();

        // Act
        var result = message.EvaluateHasParticipantExpression(message.Recipients.First().Address);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasParticipant_is_false_for_recipient_when_relationship_to_sender_is_decomposed()
    {
        // Arrange
        var message = TestData.CreateMessageWithTwoRecipients();
        message.DecomposeFor(message.Recipients.First().Address, message.CreatedBy, AnonymizedAddress);

        // Act
        var result = message.EvaluateHasParticipantExpression(message.Recipients.First().Address);

        // Assert
        result.Should().BeFalse();
    }

    #endregion
}

file static class MessageExtensions
{
    public static bool EvaluateWasExchangedBetweenExpression(this Message message, IdentityAddress identityAddressOne, IdentityAddress identityAddressTwo)
    {
        var expression = Message.WasExchangedBetween(identityAddressOne, identityAddressTwo);
        return expression.Compile()(message);
    }

    public static bool EvaluateHasParticipantExpression(this Message message, IdentityAddress address)
    {
        var expression = Message.HasParticipant(address);
        return expression.Compile()(message);
    }
}
