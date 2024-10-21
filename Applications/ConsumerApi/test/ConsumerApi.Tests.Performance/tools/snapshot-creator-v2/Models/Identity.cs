namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

public record Identity(int Id, long AvailableRelationships, string PoolAlias)
{
    public List<RelationshipAndMessages> RelationshipAndMessages { get; } = [];
    public bool HasAvailableRelationships => AvailableRelationships > 0;

    public long AvailableRelationships { get; private set; } = AvailableRelationships;

    public long DecrementAvailableRelationships() =>
        AvailableRelationships == 0 ? throw new InvalidOperationException(IDENTITY_NO_MORE_RELATIONSHIPS_AVAILABLE) : AvailableRelationships--;
}
