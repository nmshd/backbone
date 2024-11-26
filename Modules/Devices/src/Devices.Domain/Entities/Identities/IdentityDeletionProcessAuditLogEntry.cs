using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class IdentityDeletionProcessAuditLogEntry : Entity
{
    // ReSharper disable once UnusedMember.Local
    private IdentityDeletionProcessAuditLogEntry()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        ProcessId = null!;
        IdentityAddressHash = null!;
    }

    private IdentityDeletionProcessAuditLogEntry(IdentityDeletionProcessId? processId, MessageKey messageKey, byte[] identityAddressHash, byte[]? deviceIdHash, DeletionProcessStatus? oldStatus,
        DeletionProcessStatus? newStatus, Dictionary<string, string>? additionalData = null)
    {
        Id = IdentityDeletionProcessAuditLogEntryId.Generate();
        ProcessId = processId;
        CreatedAt = SystemTime.UtcNow;
        MessageKey = messageKey;
        IdentityAddressHash = identityAddressHash;
        DeviceIdHash = deviceIdHash;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        AdditionalData = additionalData;
    }

    public IdentityDeletionProcessAuditLogEntryId Id { get; }
    public IdentityDeletionProcessId? ProcessId { get; }
    public DateTime CreatedAt { get; }
    public MessageKey MessageKey { get; }
    public byte[] IdentityAddressHash { get; }
    public byte[]? DeviceIdHash { get; }
    public DeletionProcessStatus? OldStatus { get; }
    public DeletionProcessStatus NewStatus { get; }
    public Dictionary<string, string>? AdditionalData { get; }
    public string? UsernameHashesBase64 { get; private set; }

    public static IdentityDeletionProcessAuditLogEntry ProcessStartedByOwner(IdentityDeletionProcessId processId, IdentityAddress identityAddress, DeviceId deviceId)
    {
        return new IdentityDeletionProcessAuditLogEntry(
            processId,
            MessageKey.StartedByOwner,
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
            MessageKey.StartedBySupport,
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
            MessageKey.Approved,
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
            MessageKey.Rejected,
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
            MessageKey.CancelledByOwner,
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
            MessageKey.CancelledBySupport,
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
            MessageKey.CancelledAutomatically,
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

    public static IdentityDeletionProcessAuditLogEntry DataDeleted(IdentityAddress identityAddress, string aggregateType)
    {
        return new IdentityDeletionProcessAuditLogEntry(
            null,
            MessageKey.DataDeleted,
            Hasher.HashUtf8(identityAddress.Value),
            null,
            DeletionProcessStatus.Deleting,
            DeletionProcessStatus.Deleting,
            new Dictionary<string, string>
            {
                { "aggregateType", aggregateType }
            }
        );
    }

    public static IdentityDeletionProcessAuditLogEntry DeletionCompleted(IdentityAddress identityAddress)
    {
        return new IdentityDeletionProcessAuditLogEntry(
            null,
            MessageKey.DeletionCompleted,
            Hasher.HashUtf8(identityAddress.Value),
            null,
            DeletionProcessStatus.Deleting,
            null
        );
    }

    public void AssociateUsernames(IEnumerable<Username> usernames)
    {
        var hashedUsernames = usernames.Select(u => Hasher.HashUtf8(u.Value.Trim()));
        var hashedUsernamesInBase64 = hashedUsernames.Select(Convert.ToBase64String);
        var concatenatedHashedUsernamesInBase64 = string.Join("", hashedUsernamesInBase64);

        UsernameHashesBase64 = concatenatedHashedUsernamesInBase64;
    }
}

public enum MessageKey
{
    StartedByOwner = 1,
    StartedBySupport = 2,
    Approved = 3,
    Rejected = 4,
    CancelledByOwner = 5,
    CancelledBySupport = 6,
    CancelledAutomatically = 7,
    ApprovalReminder1Sent = 8,
    ApprovalReminder2Sent = 9,
    ApprovalReminder3Sent = 10,
    GracePeriodReminder1Sent = 11,
    GracePeriodReminder2Sent = 12,
    GracePeriodReminder3Sent = 13,
    DataDeleted = 14,
    DeletionCompleted = 15
}
