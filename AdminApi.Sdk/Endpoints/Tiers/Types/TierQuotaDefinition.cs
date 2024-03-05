using Backbone.AdminApi.Sdk.Endpoints.Identities.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Tiers.Types;

public class TierQuotaDefinition
{
    public required string Id { get; set; }
    public required Metric Metric { get; set; }
    public required int Max { get; set; }
    public required string Period { get; set; }
}
