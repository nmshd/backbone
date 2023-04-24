namespace Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

public class Tier
{
    public Tier(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public string Id { get; }

    public string Name { get; }
}