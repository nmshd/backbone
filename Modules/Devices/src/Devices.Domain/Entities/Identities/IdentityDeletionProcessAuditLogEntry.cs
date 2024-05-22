using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class IdentityDeletionProcessAuditLogEntry
{
    // ReSharper disable once UnusedMember.Local
    private IdentityDeletionProcessAuditLogEntry()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        ProcessId = null!;
        IdentityAddressHash = null!;
    }

    private IdentityDeletionProcessAuditLogEntry(IdentityDeletionProcessId processId, MessageKey messageKey, byte[] identityAddressHash, byte[]? deviceIdHash, DeletionProcessStatus? oldStatus,
        DeletionProcessStatus newStatus)
    {
        Id = IdentityDeletionProcessAuditLogEntryId.Generate();
        ProcessId = processId;
        CreatedAt = SystemTime.UtcNow;
        MessageKey = messageKey;
        IdentityAddressHash = identityAddressHash;
        DeviceIdHash = deviceIdHash;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }

    public IdentityDeletionProcessAuditLogEntryId Id { get; }
    public IdentityDeletionProcessId ProcessId { get; }
    public DateTime CreatedAt { get; }
    public MessageKey MessageKey { get; }
    public byte[] IdentityAddressHash { get; }
    public byte[]? DeviceIdHash { get; }
    public DeletionProcessStatus? OldStatus { get; }
    public DeletionProcessStatus NewStatus { get; }

    public static IdentityDeletionProcessAuditLogEntry ProcessStartedByOwner(IdentityDeletionProcessId processId, IdentityAddress identityAddress, DeviceId deviceId)
    {
        return new IdentityDeletionProcessAuditLogEntry(
            processId,
            MessageKey.ProcessStartedByOwner,
            Hasher.HashUtf8(identityAddress),
            Hasher.HashUtf8(deviceId),
            null,
            DeletionProcessStatus.Approved
        );
    }

    public static IdentityDeletionProcessAuditLogEntry ProcessStartedBySupport(IdentityDeletionProcessId processId, IdentityAddress identityAddress)
    {
        return new IdentityDeletionProcessAuditLogEntry(
            processId,
            MessageKey.ProcessStartedBySupport,
            Hasher.HashUtf8(identityAddress.Value),
            null,
            null,
            DeletionProcessStatus.WaitingForApproval
        );
    }

    public static IdentityDeletionProcessAuditLogEntry ProcessApproved(IdentityDeletionProcessId processId, IdentityAddress identityAddress, DeviceId deviceId)
    {
        return new IdentityDeletionProcessAuditLogEntry(
            processId,
            MessageKey.ProcessApproved,
            Hasher.HashUtf8(identityAddress.Value),
            Hasher.HashUtf8(deviceId),
            DeletionProcessStatus.WaitingForApproval,
            DeletionProcessStatus.Approved
        );
    }

    public static IdentityDeletionProcessAuditLogEntry ProcessRejected(IdentityDeletionProcessId processId, IdentityAddress identityAddress, DeviceId deviceId)
    {
        return new IdentityDeletionProcessAuditLogEntry(
            processId,
            MessageKey.ProcessRejected,
            Hasher.HashUtf8(identityAddress.Value),
            Hasher.HashUtf8(deviceId),
            DeletionProcessStatus.WaitingForApproval,
            DeletionProcessStatus.Rejected
        );
    }

    public static IdentityDeletionProcessAuditLogEntry ProcessCancelledByOwner(IdentityDeletionProcessId processId, IdentityAddress identityAddress, DeviceId deviceId)
    {
        return new IdentityDeletionProcessAuditLogEntry(
            processId,
            MessageKey.ProcessCancelledByOwner,
            Hasher.HashUtf8(identityAddress.Value),
            Hasher.HashUtf8(deviceId),
            DeletionProcessStatus.Approved,
            DeletionProcessStatus.Cancelled
        );
    }

    public static IdentityDeletionProcessAuditLogEntry ProcessCancelledBySupport(IdentityDeletionProcessId processId, IdentityAddress identityAddress)
    {
        return new IdentityDeletionProcessAuditLogEntry(
            processId,
            MessageKey.ProcessCancelledBySupport,
            Hasher.HashUtf8(identityAddress.Value),
            null,
            DeletionProcessStatus.Approved,
            DeletionProcessStatus.Cancelled
        );
    }

    public static IdentityDeletionProcessAuditLogEntry ProcessCancelledAutomatically(IdentityDeletionProcessId processId, IdentityAddress identityAddress)
    {
        return new IdentityDeletionProcessAuditLogEntry(
            processId,
            MessageKey.ProcessCancelledAutomatically,
            Hasher.HashUtf8(identityAddress.Value),
            null,
            DeletionProcessStatus.WaitingForApproval,
            DeletionProcessStatus.Cancelled
        );
    }

    public static IdentityDeletionProcessAuditLogEntry ApprovalReminder1Sent(IdentityDeletionProcessId processId, IdentityAddress identityAddress)
    {
        return new IdentityDeletionProcessAuditLogEntry(
            processId,
            MessageKey.ApprovalReminder1Sent,
            Hasher.HashUtf8(identityAddress.Value),
            null,
            DeletionProcessStatus.WaitingForApproval,
            DeletionProcessStatus.WaitingForApproval
        );
    }

    public static IdentityDeletionProcessAuditLogEntry ApprovalReminder2Sent(IdentityDeletionProcessId processId, IdentityAddress identityAddress)
    {
        return new IdentityDeletionProcessAuditLogEntry(processId,
            MessageKey.ApprovalReminder2Sent,
            Hasher.HashUtf8(identityAddress.Value),
            null,
            DeletionProcessStatus.WaitingForApproval,
            DeletionProcessStatus.WaitingForApproval
        );
    }

    public static IdentityDeletionProcessAuditLogEntry ApprovalReminder3Sent(IdentityDeletionProcessId processId, IdentityAddress identityAddress)
    {
        return new IdentityDeletionProcessAuditLogEntry(
            processId,
            MessageKey.ApprovalReminder3Sent,
            Hasher.HashUtf8(identityAddress.Value),
            null,
            DeletionProcessStatus.WaitingForApproval,
            DeletionProcessStatus.WaitingForApproval
        );
    }

    public static IdentityDeletionProcessAuditLogEntry GracePeriodReminder1Sent(IdentityDeletionProcessId processId, IdentityAddress identityAddress)
    {
        return new IdentityDeletionProcessAuditLogEntry(
            processId,
            MessageKey.GracePeriodReminder1Sent,
            Hasher.HashUtf8(identityAddress.Value),
            null,
            DeletionProcessStatus.Approved,
            DeletionProcessStatus.Approved
        );
    }

    public static IdentityDeletionProcessAuditLogEntry GracePeriodReminder2Sent(IdentityDeletionProcessId processId, IdentityAddress identityAddress)
    {
        return new IdentityDeletionProcessAuditLogEntry(
            processId,
            MessageKey.GracePeriodReminder2Sent,
            Hasher.HashUtf8(identityAddress.Value),
            null,
            DeletionProcessStatus.Approved,
            DeletionProcessStatus.Approved
        );
    }

    public static IdentityDeletionProcessAuditLogEntry GracePeriodReminder3Sent(IdentityDeletionProcessId processId, IdentityAddress identityAddress)
    {
        return new IdentityDeletionProcessAuditLogEntry(
            processId,
            MessageKey.GracePeriodReminder3Sent,
            Hasher.HashUtf8(identityAddress.Value),
            null,
            DeletionProcessStatus.Approved,
            DeletionProcessStatus.Approved
        );
    }
}

public enum MessageKey
{
    ProcessStartedByOwner = 1,
    ProcessStartedBySupport = 2,
    ProcessApproved = 3,
    ProcessRejected = 4,
    ProcessCancelledByOwner = 5,
    ProcessCancelledBySupport = 6,
    ProcessCancelledAutomatically = 7,
    ApprovalReminder1Sent = 8,
    ApprovalReminder2Sent = 9,
    ApprovalReminder3Sent = 10,
    GracePeriodReminder1Sent = 11,
    GracePeriodReminder2Sent = 12,
    GracePeriodReminder3Sent = 13
}
