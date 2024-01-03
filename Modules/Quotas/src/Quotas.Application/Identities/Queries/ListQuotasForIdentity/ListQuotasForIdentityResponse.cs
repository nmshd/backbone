using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.ListQuotasForIdentity;

public class ListQuotasForIdentityResponse : EnumerableResponseBase<QuotaGroupDTO>
{
    public ListQuotasForIdentityResponse(IEnumerable<QuotaGroupDTO> items) : base(items) { }
}

public class QuotasForIdentityDTO
{
    public List<QuotaGroupDTO> Quotas { get; }

    public QuotasForIdentityDTO(List<QuotaDTO> quotas)
    {
        Quotas = new List<QuotaGroupDTO>();

        foreach (var quota in quotas)
        {
            AddQuota(quota);
        }
    }

    private void AddQuota(QuotaDTO quota)
    {
        var uniqueMetricKeysInQuotas = Quotas.Select(q => q.MetricKey).Distinct().ToList();

        if (!uniqueMetricKeysInQuotas.Contains(quota.Metric.Key))
        {
            Quotas.Add(new QuotaGroupDTO
            {
                MetricKey = quota.Metric.Key,
                Quotas = new List<SingleQuotaDTO>()
            });
        }

        var singleQuotaDto = new SingleQuotaDTO
        {
            Source = quota.Source,
            Max = quota.Max,
            Usage = quota.Usage,
            Period = quota.Period
        };

        foreach (var quotaGroupDto in Quotas.Where(quotaGroupDto => quotaGroupDto.MetricKey == quota.Metric.Key))
            quotaGroupDto.Quotas.Add(singleQuotaDto);
    }
}

public class SingleQuotaDTO
{
    public QuotaSource Source { get; set; }
    public int Max { get; set; }
    public uint Usage { get; set; }
    public string Period { get; set; }
}

public class QuotaGroupDTO
{
    public string MetricKey { get; set; }
    public List<SingleQuotaDTO> Quotas { get; set; }
}
