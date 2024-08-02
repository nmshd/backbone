using Backbone.Modules.Devices.Domain.Entities;

namespace Backbone.Modules.Devices.Application.Clients.Commands.UpdateClient;
public class UpdateClientResponse
{
    public UpdateClientResponse(OAuthClient client, int numberOfIdentities)
    {
        ClientId = client.ClientId;
        DisplayName = client.DisplayName;
        DefaultTier = client.DefaultTier;
        CreatedAt = client.CreatedAt;
        MaxIdentities = client.MaxIdentities;
        NumberOfIdentities = numberOfIdentities;
    }

    public string ClientId { get; set; }
    public string DisplayName { get; set; }
    public string DefaultTier { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? MaxIdentities { get; set; }
    public int NumberOfIdentities { get; set; }
}
