using Enmeshed.BuildingBlocks.Domain.Errors;

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

    public DomainError? CanBeDeleted(int clientsCount, int identitiesCount)
    {
        if (clientsCount > 0)
        {
            return DomainErrors.CannotDeleteUsedDefaultTier(clientsCount);
        }

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
