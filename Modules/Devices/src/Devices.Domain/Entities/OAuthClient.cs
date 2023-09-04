namespace Backbone.Modules.Devices.Domain.Entities;
public class OAuthClient
{
    public OAuthClient(string clientId, string displayName, string tierId)
    {
        ClientId = clientId;
        DisplayName = displayName;
        TierId = tierId;
    }

    public string ClientId { get; }
    public string DisplayName { get; }
    public string TierId { get; }
}
