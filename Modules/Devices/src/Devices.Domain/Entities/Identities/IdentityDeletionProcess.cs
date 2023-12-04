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

    public static IdentityDeletionProcess CreateDeletionProcessAsSupport(IdentityAddress createdBy)
    {
        return new IdentityDeletionProcess(createdBy, DeletionProcessStatus.WaitingForApproval);
    }

    public static IdentityDeletionProcess CreateDeletionProcessAsUser(IdentityAddress createdBy, DeviceId createdByDeviceId)
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

    public IReadOnlyList<IdentityDeletionProcessAuditLogEntry> AuditLog => _auditLog;

    public DateTime? ApprovedAt { get; private set; }
    public DeviceId? ApprovedByDevice { get; private set; }

    public DateTime? GracePeriodEndsAt { get; private set; }

    public bool IsActive()
    {
        return Status is DeletionProcessStatus.Approved or DeletionProcessStatus.WaitingForApproval;
    }
}
