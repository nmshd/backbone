using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class IdentityDeletionProcess
{
    private readonly List<IdentityDeletionProcessAuditLogEntry> _auditLog;

    public static IdentityDeletionProcess Create(IdentityAddress createdBy, DeviceId createdByDevice, IHasher hasher)
    {
        return new IdentityDeletionProcess(hasher.HashUtf8(createdBy), hasher.HashUtf8(createdByDevice));
    }

    public static IdentityDeletionProcess Create(Device device, IHasher hasher)
    {
        return new IdentityDeletionProcess(hasher.HashUtf8(device.IdentityAddress), hasher.HashUtf8(device.Id));
    }

    private IdentityDeletionProcess(byte[] identityAddressHash, byte[] deviceIdHash)
    {
        Id = IdentityDeletionProcessId.Generate();
        Status = DeletionProcessStatus.WaitingForApproval;
        CreatedAt = SystemTime.UtcNow;

        _auditLog = new List<IdentityDeletionProcessAuditLogEntry>
        {
            IdentityDeletionProcessAuditLogEntry.ProcessStarted(Id, identityAddressHash, deviceIdHash)
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
