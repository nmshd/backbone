namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;

public class Tier
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool CanBeManuallyAssigned { get; set; }
    public bool CanBeUsedAsDefaultForClient { get; set; }
}
