using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.PoolsFile;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Application.RelationshipDistributor;

public interface IRelationshipDistributor
{
    void Distribute(IList<PoolEntry> pools);
}
