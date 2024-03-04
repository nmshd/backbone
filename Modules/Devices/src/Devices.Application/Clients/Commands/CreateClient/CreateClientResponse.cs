using Backbone.Modules.Devices.Domain.Entities;

namespace Backbone.Modules.Devices.Application.Clients.Commands.CreateClient;

public class CreateClientResponse
{
    public CreateClientResponse(OAuthClient client, string clientSecret)
    {
        ClientId = client.ClientId;
        DisplayName = client.DisplayName;
        ClientSecret = clientSecret;
        DefaultTier = client.DefaultTier;
        CreatedAt = client.CreatedAt;
        MaxIdentities = client.MaxIdentities;
    }

    public string ClientId { get; set; }
    public string DisplayName { get; set; }
    public string ClientSecret { get; set; }
    public string DefaultTier { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? MaxIdentities { get; set; }
}
