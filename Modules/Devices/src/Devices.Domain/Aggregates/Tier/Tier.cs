using Backbone.Modules.Devices.Domain.Entities;

namespace Backbone.Modules.Devices.Domain.Aggregates.Tier;

public class Tier
{
    public Tier(TierName name)
    {
        Id = TierId.Generate();
        Name = name;
    }

    public TierId Id { get; }
    public TierName Name { get; }
    public ICollection<Identity> Identities { get; internal set; }
}
