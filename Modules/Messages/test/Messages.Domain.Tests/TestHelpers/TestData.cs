using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.UnitTestTools.Data;

namespace Backbone.Modules.Messages.Domain.Tests.TestHelpers;

public static class TestData
{
    public static Message CreateMessageWithOneRecipient(IdentityAddress? senderAddress = null, IdentityAddress? recipientAddress = null)
    {
        senderAddress ??= TestDataGenerator.CreateRandomIdentityAddress();
        recipientAddress ??= TestDataGenerator.CreateRandomIdentityAddress();

        var recipient = new RecipientInformation(recipientAddress, []);
        return new Message(senderAddress, TestDataGenerator.CreateRandomDeviceId(), [], [], [recipient]);
    }

    public static Message CreateMessageWithTwoRecipients(IdentityAddress? senderAddress = null, IdentityAddress? recipient1Address = null, IdentityAddress? recipient2Address = null)
    {
        senderAddress ??= TestDataGenerator.CreateRandomIdentityAddress();
        recipient1Address ??= TestDataGenerator.CreateRandomIdentityAddress();
        recipient2Address ??= TestDataGenerator.CreateRandomIdentityAddress();

        return new Message(senderAddress, TestDataGenerator.CreateRandomDeviceId(), [], [], [new RecipientInformation(recipient1Address, []), new RecipientInformation(recipient2Address, [])]);
    }
}
