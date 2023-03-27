namespace Backbone.Modules.Devices.Domain.Aggregates.Tier;

public class Tier
{
    public Tier(TierName name)
    {
        Id = TierId.Generate();
        Name = name;
    }


    public TierId Id { get; private set; }
    public TierName Name { get; private set; }
}
