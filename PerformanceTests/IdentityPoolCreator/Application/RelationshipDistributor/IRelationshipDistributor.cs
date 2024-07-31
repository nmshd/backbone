using Backbone.Identity.Pool.Creator.PoolsFile;

namespace Backbone.Identity.Pool.Creator.Application.RelationshipDistributor;

public interface IRelationshipDistributor
{
    public void Distribute(IList<PoolEntry> pools);
}
