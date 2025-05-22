namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;

public class AspNetUser
{
    public string Id { get; set; } = null!;
    public string DeviceId { get; set; } = null!;
    public DateTime? LastLoginAt { get; set; }
    public Device Device { get; set; } = null!;
}
