// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;

public class Identity
{
    public required string Address { get; init; }
    public required string TierId { get; init; }
    public required IdentityStatus Status { get; init; }
    public required DateTime? DeletionGracePeriodEndsAt { get; init; }
    public required string ClientId { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required byte IdentityVersion { get; init; }
    public virtual required IList<Device> Devices { get; init; }
}

public enum IdentityStatus
{
    Active = 0,
    ToBeDeleted = 1,
    Deleting = 2
}
