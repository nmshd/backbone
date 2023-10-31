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
        Status = DeletionProcessStatus.WaitingForApproval;
        CreatedAt = SystemTime.UtcNow;

        var auditLogEntry = createdByDevice == null
            ? IdentityDeletionProcessAuditLogEntry.ProcessStartedBySupport(Id, Hasher.HashUtf8(createdBy))
            : IdentityDeletionProcessAuditLogEntry.ProcessStartedByOwner(Id, Hasher.HashUtf8(createdBy), Hasher.HashUtf8(createdByDevice));

        _auditLog = new List<IdentityDeletionProcessAuditLogEntry>
        {
            auditLogEntry
        };
    }

    public IdentityDeletionProcessId Id { get; }
    public DeletionProcessStatus Status { get; internal set; }
    public DateTime CreatedAt { get; }

    public IReadOnlyList<IdentityDeletionProcessAuditLogEntry> AuditLog => _auditLog;

    public bool IsActive()
    {
        return Status == DeletionProcessStatus.WaitingForApproval || Status == DeletionProcessStatus.Approved;
    }

    internal bool IsApproved()
    {
        return Status == DeletionProcessStatus.Approved;
    }
}
