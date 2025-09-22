// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;

public class Device
{
    public required string Id { get; init; }
    public virtual required AspNetUser User { get; init; }
    public virtual required Identity Identity { get; init; }
    public virtual required DateTime CreatedAt { get; init; }
}
