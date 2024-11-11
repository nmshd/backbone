using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Enums;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

public record IdentityConfiguration(int Address, IdentityPoolType IdentityPoolType, PoolConfiguration PoolConfiguration)
{
    public List<RelationshipAndMessages> RelationshipAndMessages { get; } = [];

    public bool HasAvailableRelationships => NumberOfRelationships > 0;

    public string PoolAlias => PoolConfiguration.Alias;

    public int NumberOfRelationships { get; private set; } = PoolConfiguration.NumberOfRelationships;

    public int DecrementAvailableRelationships() => NumberOfRelationships == 0 ? throw new InvalidOperationException(IDENTITY_NO_MORE_RELATIONSHIPS_AVAILABLE) : NumberOfRelationships--;

    public int MessagesToSendPerRelationship => PoolConfiguration.NumberOfSentMessages / PoolConfiguration.NumberOfRelationships;
    public int ModuloSendMessages => PoolConfiguration.NumberOfSentMessages % PoolConfiguration.NumberOfRelationships;
    public int NumberOfSentMessages { get; private set; } = PoolConfiguration.NumberOfSentMessages;

    public int NumberOfDevices => PoolConfiguration.NumberOfDevices;
    public int NumberOfRelationshipTemplates => PoolConfiguration.NumberOfRelationshipTemplates;
    public int NumberOfChallenges => PoolConfiguration.NumberOfChallenges;
}
