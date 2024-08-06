using Backbone.PerformanceSnapshotCreator.PoolsFile;

namespace Backbone.PerformanceSnapshotCreator.Application.RelationshipDistributor;

public interface IRelationshipDistributor
{
    public void Distribute(IList<PoolEntry> pools);
}
