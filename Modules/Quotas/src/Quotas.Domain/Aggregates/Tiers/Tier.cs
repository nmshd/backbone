using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

public class Tier
{
    public Tier(string id, string name)
    {
        Id = id;
        Name = name;
        Quotas = new();
    }

    public string Id { get; }
    public string Name { get; }
    public List<TierQuotaDefinition> Quotas { get; }

    public TierQuotaDefinition CreateQuota(Metric metric, int max, QuotaPeriod period)
    {
        var quotaDefinition = new TierQuotaDefinition(metric, max, period);
        Quotas.Add(quotaDefinition);

        return quotaDefinition;
    }
}