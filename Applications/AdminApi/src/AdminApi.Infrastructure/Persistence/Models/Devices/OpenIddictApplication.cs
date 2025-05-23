namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;

public class OpenIddictApplication
{
    public string Id { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string DefaultTier { get; set; } = null!;
    public int? MaxIdentities { get; set; }
}
