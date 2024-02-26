using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class IdentityDeletionProcessAuditLogEntry
{
    public static IdentityDeletionProcessAuditLogEntry ProcessStartedByOwner(IdentityDeletionProcessId processId, IdentityAddress identityAddress, DeviceId deviceId)
    {
        return new IdentityDeletionProcessAuditLogEntry(processId, "The deletion process was started by the owner. It was automatically approved.", Hasher.HashUtf8(identityAddress.StringValue), Hasher.HashUtf8(deviceId.StringValue), null, DeletionProcessStatus.Approved);
    }

    public static IdentityDeletionProcessAuditLogEntry ProcessStartedBySupport(IdentityDeletionProcessId processId, IdentityAddress identityAddress)
    {
        return new IdentityDeletionProcessAuditLogEntry(processId, "The deletion process was started by support. It is now waiting for approval.", Hasher.HashUtf8(identityAddress.StringValue), null, null, DeletionProcessStatus.WaitingForApproval);
    }

    public static IdentityDeletionProcessAuditLogEntry ProcessApproved(IdentityDeletionProcessId processId, IdentityAddress identityAddress, DeviceId deviceId)
    {
        return new IdentityDeletionProcessAuditLogEntry(processId, "The deletion process was approved.", Hasher.HashUtf8(identityAddress.StringValue), Hasher.HashUtf8(deviceId.StringValue), DeletionProcessStatus.WaitingForApproval, DeletionProcessStatus.Approved);
    }

    public static IdentityDeletionProcessAuditLogEntry ProcessCanceledAutomatically(IdentityDeletionProcessId processId, IdentityAddress identityAddress)
    {
        return new IdentityDeletionProcessAuditLogEntry(processId, "The deletion process was canceled automatically. It wasn't approved by the owner within the given time.", Hasher.HashUtf8(identityAddress.StringValue), null, DeletionProcessStatus.WaitingForApproval, DeletionProcessStatus.Canceled);
    }

    public static IdentityDeletionProcessAuditLogEntry ApprovalReminder1Sent(IdentityDeletionProcessId processId, IdentityAddress identityAddress)
    {
        return new IdentityDeletionProcessAuditLogEntry(processId, "The first approval reminder notification has been sent.", Hasher.HashUtf8(identityAddress.StringValue), null, DeletionProcessStatus.WaitingForApproval, DeletionProcessStatus.WaitingForApproval);
    }

    public static IdentityDeletionProcessAuditLogEntry ApprovalReminder2Sent(IdentityDeletionProcessId processId, IdentityAddress identityAddress)
    {
        return new IdentityDeletionProcessAuditLogEntry(processId, "The second approval reminder notification has been sent.", Hasher.HashUtf8(identityAddress.StringValue), null, DeletionProcessStatus.WaitingForApproval, DeletionProcessStatus.WaitingForApproval);
    }

    public static IdentityDeletionProcessAuditLogEntry ApprovalReminder3Sent(IdentityDeletionProcessId processId, IdentityAddress identityAddress)
    {
        return new IdentityDeletionProcessAuditLogEntry(processId, "The third approval reminder notification has been sent.", Hasher.HashUtf8(identityAddress.StringValue), null, DeletionProcessStatus.WaitingForApproval, DeletionProcessStatus.WaitingForApproval);
    }

    public static IdentityDeletionProcessAuditLogEntry GracePeriodReminder1Sent(IdentityDeletionProcessId processId, IdentityAddress identityAddress)
    {
        return new IdentityDeletionProcessAuditLogEntry(processId, "The first grace period reminder notification has been sent.", Hasher.HashUtf8(identityAddress.StringValue), null, DeletionProcessStatus.Approved, DeletionProcessStatus.Approved);
    }

    public static IdentityDeletionProcessAuditLogEntry GracePeriodReminder2Sent(IdentityDeletionProcessId processId, IdentityAddress identityAddress)
    {
        return new IdentityDeletionProcessAuditLogEntry(processId, "The second grace period reminder notification has been sent.", Hasher.HashUtf8(identityAddress.StringValue), null, DeletionProcessStatus.Approved, DeletionProcessStatus.Approved);
    }

    public static IdentityDeletionProcessAuditLogEntry GracePeriodReminder3Sent(IdentityDeletionProcessId processId, IdentityAddress identityAddress)
    {
        return new IdentityDeletionProcessAuditLogEntry(processId, "The third grace period reminder notification has been sent.", Hasher.HashUtf8(identityAddress.StringValue), null, DeletionProcessStatus.Approved, DeletionProcessStatus.Approved);
    }

    // ReSharper disable once UnusedMember.Local
    private IdentityDeletionProcessAuditLogEntry()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        ProcessId = null!;
        Message = null!;
        IdentityAddressHash = null!;
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
