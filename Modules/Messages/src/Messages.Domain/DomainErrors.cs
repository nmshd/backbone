using Backbone.BuildingBlocks.Domain.Errors;

namespace Backbone.Modules.Messages.Domain;

public static class DomainErrors
{
    public static DomainError RelationshipToRecipientNotActive(string recipient = "")
    {
        var recipientText = string.IsNullOrEmpty(recipient) ? "one of the recipients" : recipient;

        return new DomainError(
            "error.platform.validation.message.relationshipToRecipientNotActive",
            $"Cannot send message to {recipientText} because the relationship to it is not active. If it's terminated, you'll need to reactivate it to be able to send messages again.");
    }

    public static DomainError MaxNumberOfUnreceivedMessagesReached(string recipient = "")
    {
        var recipientText = string.IsNullOrEmpty(recipient) ? "one of the recipients" : recipient;

        return new DomainError(
            "error.platform.validation.message.maxNumberOfUnreceivedMessagesReached",
            $"The message could not be sent because {recipientText} already has the maximum number of unread messages from you.");
    }

    public static DomainError UnableToDecompose()
    {
        return new DomainError(
            "error.platform.validation.message.unableToDecompose",
            "This recipient cannot be decomposed. It either has already been decomposed or you are no participant of this message.");
    }
}
