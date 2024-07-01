using Backbone.BuildingBlocks.Domain.Errors;

namespace Backbone.Modules.Messages.Domain;
public static class DomainErrors
{
    public static DomainError RelationshipToRecipientNotActive(string recipient = "")
    {
        var recipientText = string.IsNullOrEmpty(recipient) ? "one of the recipients" : recipient;

        return new DomainError(
            "error.platform.validation.message.relationshipToRecipientNotActive",
            $"Cannot send message to {recipientText} because the relationship to it is not active. In order to be able to send messages again, you have to reactivate the relationship.");
    }

    public static DomainError MaxNumberOfUnreceivedMessagesReached(string recipient = "")
    {
        var recipientText = string.IsNullOrEmpty(recipient) ? "one of the recipients" : recipient;

        return new DomainError(
            "error.platform.validation.message.maxNumberOfUnreceivedMessagesReached",
            $"The message could not be sent because {recipientText} already has the maximum number of unread messages from you.");
    }
}
