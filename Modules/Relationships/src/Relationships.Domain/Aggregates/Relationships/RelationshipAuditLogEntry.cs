using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipAuditLogEntry
{
    public RelationshipAuditLogEntry(RelationshipAuditLogEntryReason reason, RelationshipStatus? oldStatus, RelationshipStatus newStatus, IdentityAddress createdBy, DeviceId createdByDevice)
    {
        Id = RelationshipAuditLogEntryId.New();
        Reason = reason;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        CreatedBy = createdBy;
        CreatedByDevice = createdByDevice;
        CreatedAt = SystemTime.UtcNow;
    }

    public RelationshipAuditLogEntryId Id { get; }
    public RelationshipAuditLogEntryReason Reason { get; }
    public RelationshipStatus? OldStatus { get; }
    public RelationshipStatus NewStatus { get; }
    public IdentityAddress CreatedBy { get; }
    public DeviceId CreatedByDevice { get; }
    public DateTime CreatedAt { get; }
}
