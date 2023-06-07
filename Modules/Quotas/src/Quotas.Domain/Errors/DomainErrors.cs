using Enmeshed.BuildingBlocks.Domain.Errors;

namespace Backbone.Modules.Quotas.Domain.Errors;

public static class DomainErrors
{
    public static DomainError TierQuotaMaxValueCannotBeLowerOrEqualToZero()
    {
        return new DomainError("error.platform.quotas.invalidValueForMaxLimitInTierQuota", "A tier quota max value cannot be lower of equal to zero.");
    }
}
