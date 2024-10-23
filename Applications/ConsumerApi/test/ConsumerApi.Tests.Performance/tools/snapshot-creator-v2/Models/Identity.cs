using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Enums;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

public record Identity(int Id, IdentityPoolType IdentityPoolType, IdentityPoolConfiguration IdentityPoolConfiguration)
{
    public List<RelationshipAndMessages> RelationshipAndMessages { get; } = [];

    public bool HasAvailableRelationships => NumberOfRelationships > 0;

    public string PoolAlias => IdentityPoolConfiguration.Alias;

    public int NumberOfRelationships { get; private set; } = IdentityPoolConfiguration.NumberOfRelationships;

    public int DecrementAvailableRelationships() => NumberOfRelationships == 0 ? throw new InvalidOperationException(IDENTITY_NO_MORE_RELATIONSHIPS_AVAILABLE) : NumberOfRelationships--;

    public int MessagesToSendPerRelationship => IdentityPoolConfiguration.NumberOfSentMessages / IdentityPoolConfiguration.NumberOfRelationships;
    public int ModuloSendMessages => IdentityPoolConfiguration.NumberOfSentMessages % IdentityPoolConfiguration.NumberOfRelationships;
}
