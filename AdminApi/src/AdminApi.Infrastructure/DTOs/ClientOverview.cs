namespace Backbone.AdminApi.Infrastructure.DTOs;
public class ClientOverview
{
    public required string ClientId { get; set; }
    public required string DisplayName { get; set; }
    public required TierDTO DefaultTier { get; set; }
    public required DateTime CreatedAt { get; set; }
    public int? MaxIdentities { get; set; }
    public required int NumberOfIdentities { get; set; }
}
