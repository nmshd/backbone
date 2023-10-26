using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class IdentityDeletionProcess
{
    private readonly List<IdentityDeletionProcessAuditLogEntry> _auditLog;

    public static IdentityDeletionProcess Create(IdentityAddress createdBy, DeviceId createdByDevice, IHasher hasher)
    {
        return new IdentityDeletionProcess(hasher.HashUtf8(createdBy), hasher.HashUtf8(createdByDevice));
    }

    public static IdentityDeletionProcess Create(IdentityAddress createdBy, IHasher hasher)
    {
        return new IdentityDeletionProcess(hasher.HashUtf8(createdBy), null);
    }

    private IdentityDeletionProcess(byte[] identityAddressHash, byte[]? deviceIdHash)
    {
        Id = IdentityDeletionProcessId.Generate();
        Status = DeletionProcessStatus.WaitingForApproval;
        CreatedAt = SystemTime.UtcNow;

        var auditLogEntry = deviceIdHash == null ? 
            IdentityDeletionProcessAuditLogEntry.ProcessStartedBySupport(Id, identityAddressHash) :
            IdentityDeletionProcessAuditLogEntry.ProcessStartedByOwner(Id, identityAddressHash, deviceIdHash);

        _auditLog = new List<IdentityDeletionProcessAuditLogEntry>
        {
            auditLogEntry
        };
    }

    public IdentityDeletionProcessId Id { get; }
    public DeletionProcessStatus Status { get; }
    public DateTime CreatedAt { get; }

    public IReadOnlyList<IdentityDeletionProcessAuditLogEntry> AuditLog => _auditLog;

    public bool IsActive()
    {
        return Status == DeletionProcessStatus.WaitingForApproval;
    }
}
