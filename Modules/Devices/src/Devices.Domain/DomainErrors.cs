﻿using Enmeshed.BuildingBlocks.Domain.Errors;

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
        var formattedReason = string.IsNullOrEmpty(reason) ? "" : $" ({reason})";
        return new DomainError("error.platform.validation.device.basicTierCannotBeDeleted",
            string.IsNullOrEmpty(reason) ? $"The Basic Tier cannot be deleted {formattedReason}." : reason);
    }

    public static DomainError CannotDeleteUsedTier(int numberOfAssignedIdentities)
    {
        return new DomainError("error.platform.validation.device.usedTierCannotBeDeleted",
            $"A Tier cannot be deleted if there are Identities assigned to it ({numberOfAssignedIdentities}) found");
    }
}
