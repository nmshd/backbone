using Backbone.PerformanceSnapshotCreator.PoolsFile;

namespace Backbone.PerformanceSnapshotCreator.Application.MessageDistributor;

public interface IMessageDistributor
{
    public void Distribute(IList<PoolEntry> pools);
}
