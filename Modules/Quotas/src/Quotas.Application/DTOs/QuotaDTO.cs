using AutoMapper;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;

namespace Backbone.Modules.Quotas.Application.DTOs;
public class QuotaDTO
{
    public QuotaDTO(string id, QuotaSource source, MetricDTO metric, int max, string period)
    {
        Id = id;
        Source = source;
        Metric = metric;
        Max = max;
        Period = period;
    }

    public string Id { get; set; }
    public QuotaSource Source { get; set; }
    public MetricDTO Metric { get; set; }
    public int Max { get; set; }
    public string Period { get; set; }
}
