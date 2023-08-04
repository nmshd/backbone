using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Domain.Errors;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Domain.Aggregates.Tier;

public class Tier
{
    public Tier(TierName name)
    {
        Id = TierId.Generate();
        Name = name;
    }

    public TierId Id { get; }
    public TierName Name { get; }

    public DomainError? CanBeDeleted(int identitiesCount)
    {
        if (identitiesCount > 0)
        {
            return DomainErrors.CannotDeleteUsedTier(identitiesCount);
        }

        if (IsBasicTier())
        {
            return DomainErrors.CannotDeleteBasicTier();
        }

        return null;
    }

    public bool IsBasicTier()
    {
        return Name == TierName.BASIC_DEFAULT_NAME;
    }
}
