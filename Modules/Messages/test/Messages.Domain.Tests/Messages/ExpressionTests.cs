using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.UnitTestTools.Extensions;
using static Backbone.Modules.Messages.Domain.Tests.TestHelpers.TestData;

namespace Backbone.Modules.Messages.Domain.Tests.Messages;

public class ExpressionTests : AbstractTestsBase
{
    #region WasExchangedBetween

    [Fact]
    public void WasExchangedBetween_with_true_assertion()
    {
        // Arrange
        var message = CreateMessageWithOneRecipient();

        // Act
        var wasExchangedBetweenSenderAndRecipient = message.EvaluateWasExchangedBetweenExpression(message.CreatedBy, message.Recipients.First().Address);
        var wasExchangedBetweenRecipientAndSender = message.EvaluateWasExchangedBetweenExpression(message.Recipients.First().Address, message.CreatedBy);

        // Assert
        wasExchangedBetweenSenderAndRecipient.ShouldBeTrue();
        wasExchangedBetweenRecipientAndSender.ShouldBeTrue();
    }

    [Fact]
    public void WasExchangedBetween_with_false_assertion()
    {
        // Arrange
        var message = CreateMessageWithOneRecipient();

        // Act
        var result = message.EvaluateWasExchangedBetweenExpression(message.CreatedBy, CreateRandomIdentityAddress());

        // Assert
        result.ShouldBeFalse();
    }

    #endregion

    #region HasParticipant

    private static readonly IdentityAddress ANONYMIZED_ADDRESS = CreateRandomIdentityAddress();

    [Fact]
    public void HasParticipant_is_true_for_sender_when_no_relationship_is_decomposed()
    {
        // Arrange
        var message = CreateMessageWithOneRecipient();

        // Act
        var result = message.EvaluateHasParticipantExpression(message.CreatedBy);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void HasParticipant_is_true_for_sender_when_relationship_to_at_least_one_recipient_is_not_decomposed()
    {
        // Arrange
        var message = CreateMessageWithTwoRecipients();
        message.DecomposeFor(message.CreatedBy, message.Recipients.First().Address, ANONYMIZED_ADDRESS);

        // Act
        var result = message.EvaluateHasParticipantExpression(message.CreatedBy);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void HasParticipant_is_false_for_sender_when_relationship_to_all_recipients_are_decomposed()
    {
        // Arrange
        var message = CreateMessageWithTwoRecipients();
        message.DecomposeFor(message.CreatedBy, message.Recipients.First().Address, ANONYMIZED_ADDRESS);
        message.DecomposeFor(message.CreatedBy, message.Recipients.Second().Address, ANONYMIZED_ADDRESS);

        // Act
        var result = message.EvaluateHasParticipantExpression(message.CreatedBy);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void HasParticipant_is_true_for_recipient_when_relationship_to_sender_is_not_decomposed()
    {
        // Arrange
        var message = CreateMessageWithTwoRecipients();

        // Act
        var result = message.EvaluateHasParticipantExpression(message.Recipients.First().Address);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void HasParticipant_is_false_for_recipient_when_relationship_to_sender_is_decomposed()
    {
        // Arrange
        var message = CreateMessageWithTwoRecipients();
        message.DecomposeFor(message.Recipients.First().Address, message.CreatedBy, ANONYMIZED_ADDRESS);

        // Act
        var result = message.EvaluateHasParticipantExpression(message.Recipients.First().Address);

        // Assert
        result.ShouldBeFalse();
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
