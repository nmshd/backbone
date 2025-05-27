// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;

public class IdentityDeletionProcessAuditLogEntry
{
    public required string Id { get; init; }
    public required DeletionProcessStatus? OldStatus { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DeletionProcessStatus? NewStatus { get; init; }
    public required byte[] IdentityAddressHash { get; init; }
    public required string MessageKey { get; init; }
}

public enum DeletionProcessStatus
{
    WaitingForApproval = 0,
    Approved = 1,
    Cancelled = 2,
    Rejected = 3,
    Deleting = 10
}
