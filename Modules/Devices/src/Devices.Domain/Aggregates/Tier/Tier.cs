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

    public bool IsBasicTier()
    {
        return Name == TierName.BASIC_DEFAULT_NAME;
    }
}
