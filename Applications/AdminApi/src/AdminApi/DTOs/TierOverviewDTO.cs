namespace Backbone.AdminApi.DTOs;

public class TierOverviewDTO
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required bool CanBeUsedAsDefaultForClient { get; set; }
    public required bool CanBeManuallyAssigned { get; set; }
    public required int NumberOfIdentities { get; set; }
}
