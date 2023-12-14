using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class IdentityDeletionProcess
{
    private readonly List<IdentityDeletionProcessAuditLogEntry> _auditLog;

    // EF Core needs the empty constructor
#pragma warning disable CS8618
    // ReSharper disable once UnusedMember.Local
    private IdentityDeletionProcess()
#pragma warning restore CS8618
    {
    }

    public static IdentityDeletionProcess StartAsSupport(IdentityAddress createdBy)
    {
        return new IdentityDeletionProcess(createdBy, DeletionProcessStatus.WaitingForApproval);
    }

    public static IdentityDeletionProcess StartAsOwner(IdentityAddress createdBy, DeviceId createdByDeviceId)
    {
        return new IdentityDeletionProcess(createdBy, createdByDeviceId);
    }

    private IdentityDeletionProcess(IdentityAddress createdBy, DeletionProcessStatus status)
    {
        Id = IdentityDeletionProcessId.Generate();
        CreatedAt = SystemTime.UtcNow;
        Status = status;

        _auditLog = new List<IdentityDeletionProcessAuditLogEntry>
        {
            IdentityDeletionProcessAuditLogEntry.ProcessStartedBySupport(Id, createdBy)
        };
    }

    private IdentityDeletionProcess(IdentityAddress createdBy, DeviceId createdByDevice)
    {
        Id = IdentityDeletionProcessId.Generate();
        CreatedAt = SystemTime.UtcNow;

        Approve(createdByDevice);

        _auditLog = new List<IdentityDeletionProcessAuditLogEntry>
        {
            IdentityDeletionProcessAuditLogEntry.ProcessStartedByOwner(Id, createdBy, createdByDevice)
        };
    }

    private void Approve(DeviceId createdByDevice)
    {
        Status = DeletionProcessStatus.Approved;
        ApprovedAt = SystemTime.UtcNow;
        ApprovedByDevice = createdByDevice;
        GracePeriodEndsAt = SystemTime.UtcNow.AddDays(IdentityDeletionConfiguration.LengthOfGracePeriod);
    }

    public IdentityDeletionProcessId Id { get; }
    public DeletionProcessStatus Status { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? ApprovalReminder1SentAt { get; private set; }
    public DateTime? ApprovalReminder2SentAt { get; private set; }
    public DateTime? ApprovalReminder3SentAt { get; private set; }

    public IReadOnlyList<IdentityDeletionProcessAuditLogEntry> AuditLog => _auditLog;

    public DateTime? ApprovedAt { get; private set; }
    public DeviceId? ApprovedByDevice { get; private set; }

    public DateTime? GracePeriodEndsAt { get; private set; }

    public bool IsActive()
    {
        return Status is DeletionProcessStatus.Approved or DeletionProcessStatus.WaitingForApproval;
    }

    public void ApprovalReminder1Sent(IdentityAddress identityAddress)
    {
        ApprovalReminder1SentAt = SystemTime.UtcNow;
        _auditLog.Add(IdentityDeletionProcessAuditLogEntry.ApprovalReminder1Sent(Id, identityAddress));
    }

    public void ApprovalReminder2Sent(IdentityAddress identityAddress)
    {
        ApprovalReminder2SentAt = SystemTime.UtcNow;
        _auditLog.Add(IdentityDeletionProcessAuditLogEntry.ApprovalReminder2Sent(Id, identityAddress));
    }

    public void ApprovalReminder3Sent(IdentityAddress identityAddress)
    {
        ApprovalReminder3SentAt = SystemTime.UtcNow;
        _auditLog.Add(IdentityDeletionProcessAuditLogEntry.ApprovalReminder3Sent(Id, identityAddress));
    }
}
