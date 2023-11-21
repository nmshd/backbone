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

    public IdentityDeletionProcess(IdentityAddress createdBy, DeviceId createdByDevice)
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
    public DeletionProcessStatus Status { get; internal set; }
    public DateTime DeletionStartedAt { get; private set; }
    public DateTime CreatedAt { get; }

    public IReadOnlyList<IdentityDeletionProcessAuditLogEntry> AuditLog => _auditLog;

    public DateTime? ApprovedAt { get; private set; }
    public DeviceId? ApprovedByDevice { get; private set; }

    public DateTime? GracePeriodEndsAt { get; private set; }

    public bool IsActive()
    {
        return Status is DeletionProcessStatus.Approved;
    }

    internal void DeletionStarted()
    {
        Status = DeletionProcessStatus.Deleting;
        DeletionStartedAt = SystemTime.UtcNow;
    }
}
