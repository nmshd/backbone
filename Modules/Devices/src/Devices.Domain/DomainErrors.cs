using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.Modules.Devices.Domain.Aggregates.Relationships;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Domain;

public static class DomainErrors
{
    public static DomainError InvalidTierName(string reason = "")
    {
        var formattedReason = string.IsNullOrEmpty(reason) ? "" : $" ({reason})";
        return new DomainError("error.platform.validation.invalidTierName",
            string.IsNullOrEmpty(reason) ? $"The Tier Name is invalid{formattedReason}." : reason);
    }

    public static DomainError InvalidDeviceCommunicationLanguage()
    {
        return new DomainError("error.platform.validation.invalidDeviceCommunicationLanguage", "The Device Communication Language must be a valid two letter ISO code");
    }

    public static DomainError InvalidPnsPlatform(string reason = "")
    {
        var formattedReason = string.IsNullOrEmpty(reason) ? "" : $" ({reason})";
        return new DomainError("error.platform.validation.invalidPnsPlatform",
            string.IsNullOrEmpty(reason) ? $"The Push Notification Service Platform is invalid{formattedReason}." : reason);
    }

    public static DomainError CannotDeleteBasicTier()
    {
        return new DomainError("error.platform.validation.device.basicTierCannotBeDeleted", "The 'Basic' Tier cannot be deleted.");
    }

    public static DomainError CannotDeleteQueuedForDeletionTier()
    {
        return new DomainError("error.platform.validation.device.queuedForDeletionTierCannotBeDeleted", "The 'Queued for Deletion' Tier cannot be deleted.");
    }

    public static DomainError CannotChangeTierQueuedForDeletion()
    {
        return new DomainError("error.platform.validation.device.queuedForDeletionTierCannotBeManuallyAssignedOrUnassigned",
            "The Identity's Tier cannot be be changed from or to the 'Queued for Deletion' Tier.");
    }

    public static DomainError CannotDeleteUsedTier(string reason = "")
    {
        var formattedReason = string.IsNullOrEmpty(reason) ? "" : $" ({reason})";
        return new DomainError("error.platform.validation.device.usedTierCannotBeDeleted",
            string.IsNullOrEmpty(reason) ? $"The Tier cannot be deleted {formattedReason}" : reason);
    }

    public static DomainError OnlyOneActiveDeletionProcessAllowed()
    {
        return new DomainError("error.platform.validation.device.onlyOneActiveDeletionProcessAllowed", "Only one active deletion process is allowed.");
    }

    public static DomainError DeletionProcessMustBeInStatus(params DeletionProcessStatus[] deletionProcessStatus)
    {
        var statusList = string.Join("', '", deletionProcessStatus);
        return new DomainError("error.platform.validation.device.deletionProcessIsNotInRequiredStatus",
            $"The deletion process must be in status [{statusList}] for the operation to continue, but it was in another status.");
    }

    public static DomainError GracePeriodHasNotYetExpired()
    {
        return new DomainError("error.platform.validation.device.gracePeriodHasNotYetExpired",
            "The deletion via this deletion process cannot be started because the grace period has not yet expired.");
    }

    public static DomainError GracePeriodHasAlreadyExpired()
    {
        return new DomainError("error.platform.validation.device.gracePeriodHasAlreadyExpired",
            "You cannot perform this action, because the grace period of this deletion process has already expired.");
    }

    public static DomainError DeletionProcessMustBePastDueApproval()
    {
        return new DomainError("error.platform.validation.device.noDeletionProcessIsPastDueApproval", "No deletion process is past due approval.");
    }

    public static DomainError MaxIdentitiesLessThanCurrentIdentities(int requestedMaxIdentities, int identitiesCount)
    {
        return new DomainError("error.platform.validation.device.maxIdentitiesLessThanCurrentIdentities",
            $"The requested maximum number of identities {requestedMaxIdentities} is less than the current number of identities {identitiesCount}.");
    }

    public static DomainError RelationshipToRecipientIsNotInCorrectStatus(IEnumerable<RelationshipStatus> validStatuses)
    {
        return new DomainError("error.platform.validation.notification.relationshipToRecipientIsNotInCorrectStatus",
            $"The relationship to the recipient is not in the correct status for this operation. The valid statuses are: {string.Join(", ", validStatuses)}.");
    }
}
