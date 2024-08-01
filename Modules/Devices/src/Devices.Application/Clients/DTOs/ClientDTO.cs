namespace Backbone.Modules.Devices.Application.Clients.DTOs;
public class ClientDTO
{
    public required string ClientId { get; set; }
    public required string DisplayName { get; set; }
    public required string DefaultTier { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required int NumberOfIdentities { get; set; }
    public int? MaxIdentities { get; set; }
}
