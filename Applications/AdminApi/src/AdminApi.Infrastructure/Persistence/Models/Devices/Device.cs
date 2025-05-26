// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;

public class Device
{
    public required string Id { get; init; }
    public required AspNetUser User { get; init; }
    public required Identity Identity { get; init; }
    public required DateTime CreatedAt { get; init; }
}
