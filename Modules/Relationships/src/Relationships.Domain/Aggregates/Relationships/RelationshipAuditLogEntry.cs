using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipAuditLogEntry : Entity
{
    // ReSharper disable once UnusedMember.Local
    private RelationshipAuditLogEntry()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        CreatedBy = null!;
        CreatedByDevice = null!;
    }

    public RelationshipAuditLogEntry(RelationshipAuditLogEntryReason reason, RelationshipStatus? oldStatus, RelationshipStatus newStatus, IdentityAddress createdBy, DeviceId? createdByDevice)
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
    public IdentityAddress CreatedBy { get; private set; }
    public DeviceId? CreatedByDevice { get; }
    public DateTime CreatedAt { get; }

    public void AnonymizeIdentity(IdentityAddress anonymousIdentity)
    {
        CreatedBy = anonymousIdentity;
    }
}
