using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Enmeshed.BuildingBlocks.Domain.Errors;

namespace Backbone.Modules.Quotas.Domain;
public static class DomainErrors
{
    public static DomainError UnsupportedMetricKey()
    {
        return new DomainError("error.platform.quotas.unsupportedMetricKey", $"The given metric key is not supported. The supported metric keys are: [{string.Join(", ", MetricKey.GetSupportedMetricKeyValues())}].");
    }

    public static DomainError TierQuotaMaxValueCannotBeLowerOrEqualToZero()
    {
        return new DomainError("error.platform.quotas.invalidValueForMaxLimitInTierQuota", "A tier quota max value cannot be lower of equal to zero.");
    }

    public static DomainError TierQuotaNotFoundOnIdentity(string tierQuotaDefinitionId, string address)
    {
        return new DomainError("error.platform.quotas.tierQuotaNotFoundOnIdentity", $"The Tier Quota with Definition Id '{tierQuotaDefinitionId}' does not exist for Identity '{address}'.");
    }
}
