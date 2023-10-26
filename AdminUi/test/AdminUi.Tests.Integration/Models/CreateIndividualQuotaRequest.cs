namespace Backbone.AdminUi.Tests.Integration.Models;

public class CreateIndividualQuotaRequest
{
    public string MetricKey { get; set; }
    public int Max { get; set; }
    public string Period { get; set; }
}
