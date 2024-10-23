namespace Backbone.ConsumerApi.Sdk.Endpoints.Quotas.Types;

public class QuotaGroup
{
    public required string MetricKey { get; set; }
    public required List<SingleQuota> Quotas { get; set; }
}

public class SingleQuota
{
    public required string Source { get; set; }
    public required string MetricKey { get; set; }
    public required int Max { get; set; }
    public required uint Usage { get; set; }
    public required string Period { get; set; }
}
