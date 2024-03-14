using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Domain;

public static class DomainErrors
{
    public static DomainError InvalidTierName(string reason = "")
    {
        var formattedReason = string.IsNullOrEmpty(reason) ? "" : $" ({reason})";
        return new DomainError("error.platform.validation.invalidTierName",
            string.IsNullOrEmpty(reason) ? $"The Tier Name is invalid {formattedReason}." : reason);
    }

    public static DomainError InvalidPnsPlatform(string reason = "")
    {
        var formattedReason = string.IsNullOrEmpty(reason) ? "" : $" ({reason})";
        return new DomainError("error.platform.validation.invalidPnsPlatform",
            string.IsNullOrEmpty(reason) ? $"The Push Notification Service Platform is invalid {formattedReason}." : reason);
    }

    public static DomainError CannotDeleteBasicTier(string reason = "")
    {
        return new DomainError("error.platform.validation.device.basicTierCannotBeDeleted", "The 'Basic' Tier cannot be deleted.");
    }

    public static DomainError CannotDeleteQueuedForDeletionTier()
    {
        return new DomainError("error.platform.validation.device.queuedForDeletionTierCannotBeDeleted", "The 'Queued for Deletion' Tier cannot be deleted.");
    }

    public static DomainError CannotChangeTierQueuedForDeletion()
    {
        return new DomainError("error.platform.validation.device.queuedForDeletionTierCannotBeManuallyAssignedOrUnassigned", "The Identity's Tier cannot be be changed from or to the 'Queued for Deletion' Tier.");
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

    public static DomainError DeletionProcessMustBeInStatus(DeletionProcessStatus deletionProcessStatus)
    {
        return new DomainError($"error.platform.validation.device.deletionProcessMustBeInStatus{deletionProcessStatus}", $"The deletion process must be in status '{deletionProcessStatus}'.");
    }

    public static DomainError IdentityCannotBeDeleted()
    {
        return new DomainError($"error.platform.validation.device.identityCannotBeDeleted", $"The identity cannot be deleted because it has not been approved or is not past due approval.");
    }

    public static DomainError DeletionProcessMustBePastDueApproval()
    {
        return new DomainError("error.platform.validation.device.noDeletionProcessIsPastDueApproval", "No deletion process is past due approval.");
    }
}
