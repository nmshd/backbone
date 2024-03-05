using Backbone.AdminApi.Sdk.Endpoints.Identities.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Tiers.Types;

public class TierQuotaDefinition
{
    public string Id { get; set; }
    public Metric Metric { get; set; }
    public int Max { get; set; }
    public string Period { get; set; }
}
