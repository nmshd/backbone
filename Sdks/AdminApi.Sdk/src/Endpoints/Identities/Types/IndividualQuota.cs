namespace Backbone.AdminApi.Sdk.Endpoints.Identities.Types;

public class IndividualQuota
{
    public required string Id { get; set; }
    public required Metric Metric { get; set; }
    public required int Max { get; set; }
    public required string Period { get; set; }
}
