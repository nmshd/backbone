namespace Backbone.Modules.Quotas.Application.DTOs;
public class QuotaDTO
{
    public QuotaDTO(string id, string source, MetricDTO metric, int max, string period)
    {
        Id = id;
        Source = source;
        Metric = metric;
        Max = max;
        Period = period;
    }

    public string Id { get; set; }
    public string Source { get; set; }
    public MetricDTO Metric { get; set; }
    public int Max { get; set; }
    public string Period { get; set; }
}
