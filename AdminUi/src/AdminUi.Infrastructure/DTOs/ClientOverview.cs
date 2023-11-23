namespace Backbone.AdminUi.Infrastructure.DTOs;
public class ClientOverview
{
    public string ClientId { get; set; }
    public string DisplayName { get; set; }
    public TierDTO DefaultTier { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? MaxIdentities { get; set; }
    public int NumberOfIdentities { get; set; }
}
