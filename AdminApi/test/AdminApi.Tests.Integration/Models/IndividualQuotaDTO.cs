namespace Backbone.AdminApi.Tests.Integration.Models;
public class IndividualQuotaDTO
{
    public required string Id { get; set; }
    public required MetricDTO Metric { get; set; }
    public required int Max { get; set; }
    public required string Period { get; set; }
}

