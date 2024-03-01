namespace Backbone.AdminApi.Tests.Integration.Models;

public class CreateIndividualQuotaRequest
{
    public required string MetricKey { get; set; }
    public required int Max { get; set; }
    public required string Period { get; set; }
}
