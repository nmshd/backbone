namespace Backbone.Modules.Devices.Module.Features.Tiers.Shared;

public class TierDTO
{
    public TierDTO(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public string Id { get; set; }
    public string Name { get; set; }
}
