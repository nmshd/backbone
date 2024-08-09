namespace Backbone.Modules.Devices.Application.Tiers.DTOs;

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
