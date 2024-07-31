using Backbone.Identity.Pool.Creator.PoolsFile;

namespace Backbone.Identity.Pool.Creator.Application.MessageDistributor;

public interface IMessageDistributor
{
    public void Distribute(IList<PoolEntry> pools);
}
