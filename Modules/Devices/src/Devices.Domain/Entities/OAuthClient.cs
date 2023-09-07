namespace Backbone.Modules.Devices.Domain.Entities;
public class OAuthClient
{
    public OAuthClient(string clientId, string displayName, string defaultTier)
    {
        ClientId = clientId;
        DisplayName = displayName;
        DefaultTier = defaultTier;
    }

    public string ClientId { get; }
    public string DisplayName { get; }
    public string DefaultTier { get; }
}
