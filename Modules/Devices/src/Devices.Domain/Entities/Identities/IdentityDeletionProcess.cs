using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class IdentityDeletionProcess
{
    private readonly List<IdentityDeletionProcessAuditLogEntry> _auditLog;

    public static IdentityDeletionProcess Create(IdentityAddress createdBy, DeviceId createdByDevice)
    {
        return new IdentityDeletionProcess(Hasher.HashUtf8(createdBy), Hasher.HashUtf8(createdByDevice));
    }

    public static IdentityDeletionProcess Create(IdentityAddress createdBy)
    {
        return new IdentityDeletionProcess(Hasher.HashUtf8(createdBy), null);
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

public static class Hasher
{
    private static IHasher _hasher = new HasherImpl();

    public static void SetHasher(IHasher hasher)
    {
        _hasher = hasher;
    }

    public static byte[] HashUtf8(string input)
    {
        return _hasher.HashUtf8(input);
    }
}

internal class HasherImpl : IHasher
{
    public byte[] HashUtf8(string input)
    {
        throw new NotImplementedException();
    }
}
