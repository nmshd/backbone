using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;

namespace Backbone.Modules.Devices.Domain.Entities;
public class OAuthClient
{
    public OAuthClient(string clientId, string displayName, TierId defaultTier, DateTime createdAt, int maxIdentities)
    {
        ClientId = clientId;
        DisplayName = displayName;
        DefaultTier = defaultTier;
        CreatedAt = createdAt;
        MaxIdentities = maxIdentities;
    }

    public string ClientId { get; }
    public string DisplayName { get; }
    public TierId DefaultTier { get; private set; }
    public DateTime CreatedAt { get; }
    public int MaxIdentities { get; private set; }

    public DomainError? ChangeDefaultTier(TierId newDefaultTier)
    {
        if (DefaultTier == newDefaultTier)
            return DomainErrors.CannotChangeClientDefaultTier("The Client already uses the provided default Tier.");

        DefaultTier = newDefaultTier;
        return null;
    }

    public DomainError? ChangeMaxIdentities(int newMaxIdentities)
    {
        if (MaxIdentities == newMaxIdentities)
            return DomainErrors.CannotChangeClientDefaultTier("The Client already uses the provided maximum number of Identities.");

        MaxIdentities = newMaxIdentities;
        return null;
    }
}
