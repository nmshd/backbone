using Backbone.Tooling;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class IdentityDeletionProcessAuditLogEntry
{
    public static IdentityDeletionProcessAuditLogEntry ProcessStartedByOwner(IdentityDeletionProcessId processId, byte[] identityAddressHash, byte[] deviceIdHash)
    {
        return new IdentityDeletionProcessAuditLogEntry(processId, "The deletion process was started by the owner.", identityAddressHash, deviceIdHash, null, DeletionProcessStatus.WaitingForApproval);
    }

    public static IdentityDeletionProcessAuditLogEntry ProcessStartedBySupport(IdentityDeletionProcessId processId, byte[] identityAddressHash)
    {
        return new IdentityDeletionProcessAuditLogEntry(processId, "The deletion process was started by a support employee.", identityAddressHash, null, null, DeletionProcessStatus.WaitingForApproval);
    }

    private IdentityDeletionProcessAuditLogEntry()
    {
        
    }

    private IdentityDeletionProcessAuditLogEntry(IdentityDeletionProcessId processId, string message, byte[] identityAddressHash, byte[]? deviceIdHash, DeletionProcessStatus? oldStatus, DeletionProcessStatus newStatus)
    {
        Id = IdentityDeletionProcessAuditLogEntryId.Generate();
        ProcessId = processId;
        CreatedAt = SystemTime.UtcNow;
        Message = message;
        IdentityAddressHash = identityAddressHash;
        DeviceIdHash = deviceIdHash;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }

    public IdentityDeletionProcessAuditLogEntryId Id { get; }
    public IdentityDeletionProcessId ProcessId { get; }
    public DateTime CreatedAt { get; }
    public string Message { get; }
    public byte[] IdentityAddressHash { get; }
    public byte[]? DeviceIdHash { get; }
    public DeletionProcessStatus? OldStatus { get; }
    public DeletionProcessStatus NewStatus { get; }
}
