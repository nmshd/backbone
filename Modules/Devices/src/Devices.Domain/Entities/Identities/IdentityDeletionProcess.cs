using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities.Identities.Hashing;
using Backbone.Tooling;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class IdentityDeletionProcess
{
    private readonly List<IdentityDeletionProcessAuditLogEntry> _auditLog;

    private IdentityDeletionProcess()
    {
    }

    public IdentityDeletionProcess(IdentityAddress createdBy, DeviceId? createdByDevice = null)
    {
        Id = IdentityDeletionProcessId.Generate();
        CreatedAt = SystemTime.UtcNow;

        IdentityDeletionProcessAuditLogEntry auditLogEntry;

        if (createdByDevice != null)
        {
            Status = DeletionProcessStatus.Approved;
            ApprovedAt = SystemTime.UtcNow;
            ApprovedByDevice = createdByDevice;

            auditLogEntry = IdentityDeletionProcessAuditLogEntry.ProcessStartedByOwner(Id, createdBy, createdByDevice);
        }
        else
        {
            Status = DeletionProcessStatus.WaitingForApproval;
            auditLogEntry = IdentityDeletionProcessAuditLogEntry.ProcessStartedBySupport(Id, Hasher.HashUtf8(createdBy));
        }

        _auditLog = new List<IdentityDeletionProcessAuditLogEntry>
        {
            auditLogEntry
        };
    }

    public IdentityDeletionProcessId Id { get; }
    public DeletionProcessStatus Status { get; }
    public DateTime CreatedAt { get; }

    public IReadOnlyList<IdentityDeletionProcessAuditLogEntry> AuditLog => _auditLog;
    
    public DateTime? ApprovedAt { get; }
    public DeviceId? ApprovedByDevice { get; }

    public bool IsActive()
    {
        return Status is DeletionProcessStatus.WaitingForApproval or DeletionProcessStatus.Approved;
    }
}
