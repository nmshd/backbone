using Backbone.Modules.Quotas.Domain.Aggregates.Identities;

namespace Backbone.Modules.Quotas.Application.DTOs;

public class QuotaDTO
{
    public QuotaDTO(Quota quota, MetricDTO metric, uint usage)
    {
        Id = quota.Id;
        Source = quota is TierQuota ? QuotaSource.Tier : QuotaSource.Individual;
        Metric = metric;
        Max = quota.Max;
        Period = quota.Period.ToString();
        Usage = usage;
    }

    public string Id { get; set; }
    public QuotaSource Source { get; set; }
    public MetricDTO Metric { get; set; }
    public int Max { get; set; }
    public uint Usage { get; set; }
    public string Period { get; set; }
}
