using Backbone.Modules.Devices.Domain.Entities;

namespace Backbone.Modules.Devices.Application.Clients.Commands.ChangeClientSecret;
public class ChangeClientSecretResponse
{
    public ChangeClientSecretResponse(OAuthClient client, string clientSecret, int numberOfIdentities)
    {
        ClientId = client.ClientId;
        DisplayName = client.DisplayName;
        ClientSecret = clientSecret;
        DefaultTier = client.DefaultTier;
        CreatedAt = client.CreatedAt;
        MaxIdentities = client.MaxIdentities;
        NumberOfIdentities = numberOfIdentities;
    }

    public string ClientId { get; set; }
    public string DisplayName { get; set; }
    public string ClientSecret { get; set; }
    public string DefaultTier { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? MaxIdentities { get; set; }
    public int NumberOfIdentities { get; set; }
}
