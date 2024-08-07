using Backbone.Modules.Devices.Domain.Entities;

namespace Backbone.Modules.Devices.Application.Clients.DTOs;

public class ClientDTO
{
    public ClientDTO(OAuthClient client, int? numberOfIdentities = null)
    {
        ClientId = client.ClientId;
        DisplayName = client.DisplayName;
        DefaultTier = client.DefaultTier;
        CreatedAt = client.CreatedAt;
        NumberOfIdentities = numberOfIdentities;
        MaxIdentities = client.MaxIdentities;
    }

    public string ClientId { get; set; }
    public string DisplayName { get; set; }
    public string DefaultTier { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? NumberOfIdentities { get; set; }
    public int? MaxIdentities { get; set; }
}
