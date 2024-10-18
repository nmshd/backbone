using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;

namespace Backbone.Modules.Messages.Application.Tests.TestHelpers;

public static class TestData
{
    public static Message CreateMessageWithOneRecipient(IdentityAddress? senderAddress = null, IdentityAddress? recipientAddress = null)
    {
        senderAddress ??= CreateRandomIdentityAddress();
        recipientAddress ??= CreateRandomIdentityAddress();

        var recipient = new RecipientInformation(recipientAddress, []);
        return new Message(senderAddress, CreateRandomDeviceId(), [], [], [recipient]);
    }

    public static Message CreateMessageWithTwoRecipients(IdentityAddress? senderAddress = null, IdentityAddress? recipient1Address = null, IdentityAddress? recipient2Address = null)
    {
        senderAddress ??= CreateRandomIdentityAddress();
        recipient1Address ??= CreateRandomIdentityAddress();
        recipient2Address ??= CreateRandomIdentityAddress();

        return new Message(senderAddress, CreateRandomDeviceId(), [], [], [new RecipientInformation(recipient1Address, []), new RecipientInformation(recipient2Address, [])]);
    }
}
