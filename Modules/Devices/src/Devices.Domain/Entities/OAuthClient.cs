using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Enmeshed.BuildingBlocks.Domain.Errors;

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

    public DomainError? ChangeDefaultTier(TierId newDefaultTier)
    {
        if (DefaultTier == newDefaultTier)
            return DomainErrors.CannotChangeClientDefaultTier("The Client already uses the provided default Tier.");

        DefaultTier = newDefaultTier;
        return null;
    }
}
