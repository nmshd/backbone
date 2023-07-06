using Enmeshed.BuildingBlocks.Domain.Errors;

namespace Backbone.Modules.Quotas.Domain.Errors;

public static class DomainErrors
{
    public static DomainError MaxValueCannotBeLowerOrEqualToZero()
    {
        return new DomainError("error.platform.quotas.invalidValueForMaxLimitInQuota", "A quota max value cannot be lower of equal to zero.");
    }
}
