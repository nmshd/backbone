namespace Backbone.Modules.Devices.Application.Clients.Commands.CreateClient;

public class CreateClientResponse
{
    public CreateClientResponse(string clientId, string displayName, string clientSecret, string defaultTier, DateTime createdAt)
    {
        ClientId = clientId;
        DisplayName = displayName;
        ClientSecret = clientSecret;
        DefaultTier = defaultTier;
        CreatedAt = createdAt;
    }

    public string ClientId { get; set; }
    public string DisplayName { get; set; }
    public string ClientSecret { get; set; }
    public string DefaultTier { get; set; }
    public DateTime CreatedAt { get; set; }
}
