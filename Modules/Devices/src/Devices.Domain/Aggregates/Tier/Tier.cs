using Backbone.BuildingBlocks.Domain.Errors;

namespace Backbone.Modules.Devices.Domain.Aggregates.Tier;

public class Tier
{
    public static readonly Tier QUEUED_FOR_DELETION = new(TierId.Create("TIR00000000000000001").Value, TierName.Create("Queued for Deletion").Value, false, false);

    public Tier(TierName name) : this(TierId.Generate(), name, true, true)
    {
    }

    private Tier(TierId id, TierName name, bool canBeUsedAsDefaultForClient, bool canBeManuallyAssigned)
    {
        Id = id;
        Name = name;
        CanBeUsedAsDefaultForClient = canBeUsedAsDefaultForClient;
        CanBeManuallyAssigned = canBeManuallyAssigned;
    }

    public TierId Id { get; }
    public TierName Name { get; }
    public bool CanBeUsedAsDefaultForClient { get; }
    public bool CanBeManuallyAssigned { get; }

    public DomainError? CanBeDeleted(int clientsCount, int identitiesCount)
    {
        if (clientsCount > 0)
            return DomainErrors.CannotDeleteUsedTier($"The Tier is used as the default Tier by one or more clients. A Tier cannot be deleted if it is the default Tier of a Client ({clientsCount} found).");

        if (identitiesCount > 0)
            return DomainErrors.CannotDeleteUsedTier($"The Tier is assigned to one or more Identities. A Tier cannot be deleted if it is assigned to an Identity ({identitiesCount} found).");

        if (IsBasicTier())
            return DomainErrors.CannotDeleteBasicTier();

        if (IsQueuedForDeletionTier())
            return DomainErrors.CannotDeleteQueuedForDeletionTier();

        return null;
    }

    public bool IsBasicTier()
    {
        return Name == TierName.BASIC_DEFAULT_NAME;
    }

    public bool IsQueuedForDeletionTier()
    {
        return Id == QUEUED_FOR_DELETION.Id;
    }
}
