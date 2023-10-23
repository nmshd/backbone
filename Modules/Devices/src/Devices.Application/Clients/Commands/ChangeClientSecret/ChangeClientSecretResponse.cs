using Backbone.Devices.Domain.Entities;

namespace Backbone.Devices.Application.Clients.Commands.ChangeClientSecret;
public class ChangeClientSecretResponse
{
    public ChangeClientSecretResponse(OAuthClient client, string clientSecret)
    {
        ClientId = client.ClientId;
        DisplayName = client.DisplayName;
        ClientSecret = clientSecret;
        DefaultTier = client.DefaultTier;
        CreatedAt = client.CreatedAt;
    }

    public string ClientId { get; set; }
    public string DisplayName { get; set; }
    public string ClientSecret { get; set; }
    public string DefaultTier { get; set; }
    public DateTime CreatedAt { get; set; }
}
