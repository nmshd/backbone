using Backbone.BuildingBlocks.Domain.Errors;

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
        return new DomainError("error.platform.validation.device.basicTierCannotBeDeleted", "The Basic Tier cannot be deleted.");
    }

    public static DomainError CannotDeleteUpForDeletionTier()
    {
        return new DomainError("error.platform.validation.device.upForDeletionTierCannotBeDeleted", "The Up For Deletion Tier cannot be deleted.");
    }

    public static DomainError CannotChangeTierUpForDeletion()
    {
        return new DomainError("error.platform.validation.device.upForDeletionTierCannotBeManuallyAssignedOrUnassigned", "The Identity's Tier cannot be be changed from or to the Up for Deletion Tier.");
    }

    public static DomainError CannotDeleteUsedTier(string reason = "")
    {
        var formattedReason = string.IsNullOrEmpty(reason) ? "" : $" ({reason})";
        return new DomainError("error.platform.validation.device.usedTierCannotBeDeleted",
            string.IsNullOrEmpty(reason) ? $"The Tier cannot be deleted {formattedReason}" : reason);
    }

    public static DomainError CannotChangeClientDefaultTier(string reason = "")
    {
        var formattedReason = string.IsNullOrEmpty(reason) ? "" : $" ({reason})";
        return new DomainError("error.platform.validation.device.clientDefaultTierCannotBeChanged",
            string.IsNullOrEmpty(reason) ? $"The Client's Default Tier cannot be changed {formattedReason}" : reason);
    }

    public static DomainError OnlyOneActiveDeletionProcessAllowed()
    {
        return new DomainError("error.platform.validation.device.onlyOneActiveDeletionProcessAllowed", "Only one active deletion process is allowed.");
    }
}
