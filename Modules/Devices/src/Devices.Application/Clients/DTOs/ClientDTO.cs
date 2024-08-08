using Backbone.Modules.Devices.Domain.Entities;

namespace Backbone.Modules.Devices.Application.Clients.DTOs;
public class ClientDTO
{
    public ClientDTO(OAuthClient client, int numberOfIdentities)
    {
        ClientId = client.ClientId;
        DisplayName = client.DisplayName;
        DefaultTier = client.DefaultTier;
        CreatedAt = client.CreatedAt;
        NumberOfIdentities = numberOfIdentities;
        MaxIdentities = client.MaxIdentities;
    }

    public ClientDTO(string clientId, string displayName, string defaultTier, DateTime createdAt, int numberOfIdentities, int? maxIdentities)
    {
        ClientId = clientId;
        DisplayName = displayName;
        DefaultTier = defaultTier;
        CreatedAt = createdAt;
        NumberOfIdentities = numberOfIdentities;
        MaxIdentities = maxIdentities;
    }

    public string ClientId { get; set; }
    public string DisplayName { get; set; }
    public string DefaultTier { get; set; }
    public DateTime CreatedAt { get; set; }
    public int NumberOfIdentities { get; set; }
    public int? MaxIdentities { get; set; }
}
