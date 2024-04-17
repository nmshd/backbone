using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;

namespace Backbone.Modules.Messages.Application;

public static class ApplicationErrors
{
    public static ApplicationError NoRelationshipToRecipientExists(string recipient = "")
    {
        var recipientText = string.IsNullOrEmpty(recipient) ? "one of the recipients" : recipient;

        return new ApplicationError(
            "error.platform.validation.message.noRelationshipToRecipientExists",
            $"Cannot send message to {recipientText} because there is no relationship to it.");
    }
}
