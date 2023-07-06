namespace AdminUi.Tests.Integration.Models;

public class CreateTierQuotaRequest
{
    public string MetricKey { get; set; }
    public int Max { get; set; }
    public string Period { get; set; }
}
