namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;

public class Identity
{
    public string Address { get; set; } = null!;
    public string TierId { get; set; } = null!;
    public IdentityStatus Status { get; set; }
    public DateTime? DeletionGracePeriodEndsAt { get; set; }
    public string ClientId { get; set; } = null!;
}

public enum IdentityStatus
{
    Active = 0,
    ToBeDeleted = 1,
    Deleting = 2
}
