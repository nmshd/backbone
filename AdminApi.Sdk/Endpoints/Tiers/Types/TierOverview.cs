namespace Backbone.AdminApi.Sdk.Endpoints.Tiers.Types;

public class TierOverview
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required int NumberOfIdentities { get; set; }
}
