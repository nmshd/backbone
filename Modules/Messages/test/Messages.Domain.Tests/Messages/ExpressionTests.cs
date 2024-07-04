using Backbone.Modules.Messages.Domain.Entities;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using Xunit;

namespace Backbone.Modules.Messages.Domain.Tests.Messages;

public class ExpressionTests : AbstractTestsBase
{
    #region WasExchangedBetween

    [Fact]
    public void WasExchangedBetween_with_true_assertion()
    {
        var identityOne = TestDataGenerator.CreateRandomIdentityAddress();
        var identityTwo = TestDataGenerator.CreateRandomIdentityAddress();
        var recipient = new RecipientInformation(identityTwo, []);
        var messageFromIdentityOneToIdentityTwo = new Message(identityOne, TestDataGenerator.CreateRandomDeviceId(), [], [], [recipient]);


        var resultOne = messageFromIdentityOneToIdentityTwo.EvaluateWasExchangedBetweenExpression(identityOne, identityTwo);
        var resultTwo = messageFromIdentityOneToIdentityTwo.EvaluateWasExchangedBetweenExpression(identityTwo, identityOne);

        Assert.True(resultOne);
        Assert.True(resultTwo);
    }

    [Fact]
    public void WasExchangedBetween_with_false_assertion()
    {
        var sender = TestDataGenerator.CreateRandomIdentityAddress();
        var recipientAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var recipient = new RecipientInformation(recipientAddress, []);
        var message = new Message(sender, TestDataGenerator.CreateRandomDeviceId(), [], [], [recipient]);


        var result = message.EvaluateWasExchangedBetweenExpression(sender, TestDataGenerator.CreateRandomIdentityAddress());

        Assert.False(result);
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
