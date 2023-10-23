using Backbone.Devices.Domain.Entities;

namespace Backbone.Devices.Application.Clients.Commands.UpdateClient;
public class UpdateClientResponse
{
    public UpdateClientResponse(OAuthClient client)
    {
        ClientId = client.ClientId;
        DisplayName = client.DisplayName;
        DefaultTier = client.DefaultTier;
        CreatedAt = client.CreatedAt;
    }

    public string ClientId { get; set; }
    public string DisplayName { get; set; }
    public string DefaultTier { get; set; }
    public DateTime CreatedAt { get; set; }
}
