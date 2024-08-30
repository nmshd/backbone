using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;

namespace Backbone.Modules.Devices.Domain.Entities;

public class OAuthClient : Entity
{
    // ReSharper disable once UnusedMember.Local
    private OAuthClient()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        ClientId = null!;
        DisplayName = null!;
        DefaultTier = null!;
    }

    public OAuthClient(string clientId, string displayName, TierId defaultTier, DateTime createdAt, int? maxIdentities)
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
    public int? MaxIdentities { get; private set; }

    public bool Update(TierId newDefaultTier, int? newMaxIdentities, int identitiesCount)
    {
        if (newMaxIdentities < identitiesCount)
            throw new DomainException(DomainErrors.MaxIdentitiesLessThanCurrentIdentities(newMaxIdentities.Value, identitiesCount));

        var hasChanges = false;

        if (DefaultTier != newDefaultTier)
        {
            hasChanges = true;
            DefaultTier = newDefaultTier;
        }

        if (MaxIdentities != newMaxIdentities)
        {
            hasChanges = true;
            MaxIdentities = newMaxIdentities;
        }

        return hasChanges;
    }
}
