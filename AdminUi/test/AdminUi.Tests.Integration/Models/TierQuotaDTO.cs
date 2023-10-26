namespace Backbone.AdminUi.Tests.Integration.Models;

public class TierQuotaDTO
{
    public string Id { get; set; }
    public MetricDTO Metric { get; set; }
    public int Max { get; set; }
    public string Period { get; set; }
}
