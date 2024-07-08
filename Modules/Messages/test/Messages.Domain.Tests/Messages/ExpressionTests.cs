using Backbone.Modules.Messages.Domain.Entities;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
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
}

file static class MessageExtensions
{
    public static bool EvaluateWasExchangedBetweenExpression(this Message message, string identityAddressOne, string identityAddressTwo)
    {
        var expression = Message.WasExchangedBetween(identityAddressOne, identityAddressTwo);
        return expression.Compile()(message);
    }
}
