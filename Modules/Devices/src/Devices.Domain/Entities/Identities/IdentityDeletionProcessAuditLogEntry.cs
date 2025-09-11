using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;
using Backbone.Tooling.Extensions;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class IdentityDeletionProcessAuditLogEntry : Entity
{
    // ReSharper disable once UnusedMember.Local
    protected IdentityDeletionProcessAuditLogEntry()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        ProcessId = null!;
        IdentityAddressHash = null!;
    }

    private IdentityDeletionProcessAuditLogEntry(MessageKey messageKey, byte[] identityAddressHash, byte[]? deviceIdHash, DeletionProcessStatus? oldStatus,
        DeletionProcessStatus? newStatus, Dictionary<string, string>? additionalData = null)
    {
        Id = IdentityDeletionProcessAuditLogEntryId.Generate();
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
    public DeletionProcessStatus? NewStatus { get; }
    public Dictionary<string, string>? AdditionalData { get; }
    public List<string>? UsernameHashesBase64 { get; private set; }

    public static IdentityDeletionProcessAuditLogEntry ProcessStartedByOwner(IdentityAddress identityAddress, DeviceId deviceId)
    {
        return new IdentityDeletionProcessAuditLogEntry(
            MessageKey.StartedByOwner,
            Hasher.HashUtf8(identityAddress),
            Hasher.HashUtf8(deviceId),
            null,
            DeletionProcessStatus.Approved
        );
    }

    public static IdentityDeletionProcessAuditLogEntry ProcessCancelledByOwner(IdentityAddress identityAddress, DeviceId deviceId)
    {
        return new IdentityDeletionProcessAuditLogEntry(
            MessageKey.CancelledByOwner,
            Hasher.HashUtf8(identityAddress.Value),
            Hasher.HashUtf8(deviceId),
            DeletionProcessStatus.Approved,
            DeletionProcessStatus.Cancelled
        );
    }

    public static IdentityDeletionProcessAuditLogEntry GracePeriodReminder1Sent(IdentityAddress identityAddress)
    {
        return new IdentityDeletionProcessAuditLogEntry(
            MessageKey.GracePeriodReminder1Sent,
            Hasher.HashUtf8(identityAddress.Value),
            null,
            DeletionProcessStatus.Approved,
            DeletionProcessStatus.Approved
        );
    }

    public static IdentityDeletionProcessAuditLogEntry GracePeriodReminder2Sent(IdentityAddress identityAddress)
    {
        return new IdentityDeletionProcessAuditLogEntry(
            MessageKey.GracePeriodReminder2Sent,
            Hasher.HashUtf8(identityAddress.Value),
            null,
            DeletionProcessStatus.Approved,
            DeletionProcessStatus.Approved
        );
    }

    public static IdentityDeletionProcessAuditLogEntry GracePeriodReminder3Sent(IdentityAddress identityAddress)
    {
        return new IdentityDeletionProcessAuditLogEntry(
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

    public static IdentityDeletionProcessAuditLogEntry ErrorDuringDeletion(IdentityAddress identityAddress, string errorMessage)
    {
        return new IdentityDeletionProcessAuditLogEntry(
            MessageKey.ErrorDuringDeletion,
            Hasher.HashUtf8(identityAddress.Value),
            null,
            DeletionProcessStatus.Deleting,
            DeletionProcessStatus.Deleting,
            new Dictionary<string, string>
            {
                { "errorMessage", errorMessage }
            }
        );
    }

    public static IdentityDeletionProcessAuditLogEntry DeletionCompleted(IdentityAddress identityAddress)
    {
        return new IdentityDeletionProcessAuditLogEntry(
            messageKey: MessageKey.DeletionCompleted,
            identityAddressHash: Hasher.HashUtf8(identityAddress.Value),
            deviceIdHash: null,
            oldStatus: DeletionProcessStatus.Deleting,
            newStatus: null
        );
    }

    public void AssociateUsernames(IEnumerable<Username> usernames)
    {
        UsernameHashesBase64 = usernames
            .Select(u => Hasher.HashUtf8(u.Value.Trim()))
            .Select(Convert.ToBase64String)
            .ToList();
    }

    public static Expression<Func<IdentityDeletionProcessAuditLogEntry, bool>> CanBeCleanedUp =>
        ((Expression<Func<IdentityDeletionProcessAuditLogEntry, bool>>)(t => t.CreatedAt.AddDays(IdentityDeletionConfiguration.Instance.AuditLogRetentionPeriodInDays) <= SystemTime.UtcNow))
        .And(BelongsToADeletedIdentity);

    // The 'UsernameHashesBase64' property is only set after the identity was deleted. This is why we can do this check - even though it's kind of hacky.
    public static Expression<Func<IdentityDeletionProcessAuditLogEntry, bool>> BelongsToADeletedIdentity => t => t.UsernameHashesBase64 != null;

    public static Expression<Func<IdentityDeletionProcessAuditLogEntry, bool>> IsAssociatedToUser(Username username)
    {
        var usernameHashBase64 = Convert.ToBase64String(Hasher.HashUtf8(username.Value.Trim()));
        return logEntry => logEntry.UsernameHashesBase64 != null && logEntry.UsernameHashesBase64.Contains(usernameHashBase64);
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
    DeletionCompleted = 15,
    ErrorDuringDeletion = 16
}
