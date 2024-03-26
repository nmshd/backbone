using Backbone.AdminApi.Sdk.Endpoints.Identities.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Tiers.Types;

public class TierDetails
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required List<TierQuotaDefinition> Quotas { get; set; }
}
