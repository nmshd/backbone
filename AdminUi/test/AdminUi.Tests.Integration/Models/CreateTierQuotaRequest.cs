namespace Backbone.AdminUi.Tests.Integration.Models;

public class CreateTierQuotaRequest
{
    public required string MetricKey { get; set; }
    public required int Max { get; set; }
    public required string Period { get; set; }
}
