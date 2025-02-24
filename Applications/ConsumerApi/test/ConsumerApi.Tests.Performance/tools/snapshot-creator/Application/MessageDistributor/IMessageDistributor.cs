using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.PoolsFile;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Application.MessageDistributor;

public interface IMessageDistributor
{
    void Distribute(IList<PoolEntry> pools);
}
