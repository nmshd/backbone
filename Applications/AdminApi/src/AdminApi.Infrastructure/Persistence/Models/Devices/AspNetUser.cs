// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;

public class AspNetUser
{
    public required string Id { get; init; }
    public required string DeviceId { get; init; }
    public required DateTime? LastLoginAt { get; init; }
    public required Device Device { get; init; }
}
