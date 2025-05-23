namespace Backbone.AdminApi.DTOs;

public class ClientOverviewDTO
{
    public required string ClientId { get; set; }
    public required string DisplayName { get; set; }
    public required TierDTO DefaultTier { get; set; }
    public required DateTime CreatedAt { get; set; }
    public int? MaxIdentities { get; set; }
    public required int NumberOfIdentities { get; set; }
}
