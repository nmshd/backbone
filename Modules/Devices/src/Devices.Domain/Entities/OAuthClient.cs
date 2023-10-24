using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;

namespace Backbone.Modules.Devices.Domain.Entities;
public class OAuthClient
{
    public OAuthClient(string clientId, string displayName, TierId defaultTier, DateTime createdAt)
    {
        ClientId = clientId;
        DisplayName = displayName;
        DefaultTier = defaultTier;
        CreatedAt = createdAt;
    }

    public string ClientId { get; }
    public string DisplayName { get; }
    public TierId DefaultTier { get; private set; }
    public DateTime CreatedAt { get; }

    public DomainError? ChangeDefaultTier(TierId newDefaultTier)
    {
        if (DefaultTier == newDefaultTier)
            return DomainErrors.CannotChangeClientDefaultTier("The Client already uses the provided default Tier.");

        DefaultTier = newDefaultTier;
        return null;
    }
}
