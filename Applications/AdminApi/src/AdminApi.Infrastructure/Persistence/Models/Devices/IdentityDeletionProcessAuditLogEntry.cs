namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;

public class IdentityDeletionProcessAuditLogEntry
{
    public string Id { get; set; } = null!;
    public DeletionProcessStatus? OldStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DeletionProcessStatus? NewStatus { get; set; }
    public byte[] IdentityAddressHash { get; set; } = null!;
    public string MessageKey { get; set; } = null!;
}

public enum DeletionProcessStatus
{
    WaitingForApproval = 0,
    Approved = 1,
    Cancelled = 2,
    Rejected = 3,
    Deleting = 10
}
