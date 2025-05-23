namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;

public class Device
{
    public string Id { get; set; } = null!;
    public AspNetUser User { get; set; } = null!;
    public Identity Identity { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
