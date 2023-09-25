using Backbone.Modules.Devices.Domain.Aggregates.Tier;

namespace Backbone.Modules.Devices.Domain.Entities;
public class OAuthClient
{
    public OAuthClient(string clientId, string displayName, TierId defaultTier)
    {
        ClientId = clientId;
        DisplayName = displayName;
        DefaultTier = defaultTier;
    }

    public string ClientId { get; }
    public string DisplayName { get; }
    public TierId DefaultTier { get; private set; }

    public void ChangeDefaultTier(TierId newDefaultTier)
    {
        DefaultTier = newDefaultTier;
    }
}
