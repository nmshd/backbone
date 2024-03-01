namespace Backbone.AdminApi.Infrastructure.DTOs;
public class TierOverview
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required int NumberOfIdentities { get; set; }
}
