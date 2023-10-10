namespace AdminUi.Infrastructure.DTOs;
public class ClientOverview
{
    public string ClientId { get; set; }
    public string DisplayName { get; set; }
    public string DefaultTierId { get; set; }
    public string DefaultTierName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int NumberOfIdentities { get; set; }
}
