namespace Backbone.AdminApi.Sdk.Endpoints.Tiers.Types;

public class CreateQuotaForTierRequest
{
    public required string MetricKey { get; set; }
    public required int Max { get; set; }
    public required string Period { get; set; }
}
