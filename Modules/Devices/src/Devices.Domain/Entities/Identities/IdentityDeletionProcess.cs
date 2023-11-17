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

        Status = DeletionProcessStatus.Approved;
        ApprovedAt = SystemTime.UtcNow;
        ApprovedByDevice = createdByDevice;
        GracePeriodEndsAt = SystemTime.UtcNow.AddDays(IdentityDeletionConfiguration.LengthOfGracePeriod);

        _auditLog = new List<IdentityDeletionProcessAuditLogEntry>
        {
            IdentityDeletionProcessAuditLogEntry.ProcessStartedByOwner(Id, createdBy, createdByDevice)
        };
    }

    public IdentityDeletionProcessId Id { get; }
    public DeletionProcessStatus Status { get; }
    public DateTime CreatedAt { get; }

    public IReadOnlyList<IdentityDeletionProcessAuditLogEntry> AuditLog => _auditLog;

    public DateTime? ApprovedAt { get; }
    public DeviceId? ApprovedByDevice { get; }

    public DateTime? GracePeriodEndsAt { get; }

    public bool IsActive()
    {
        return Status is DeletionProcessStatus.Approved;
    }
}
