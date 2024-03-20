using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;

namespace Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

public class RelationshipTemplateAllocation
{
    public RelationshipTemplateAllocation(RelationshipTemplateId relationshipTemplateId, IdentityAddress allocatedBy, DeviceId allocatedByDevice)
    {
        RelationshipTemplateId = relationshipTemplateId;
        AllocatedAt = SystemTime.UtcNow;
        AllocatedBy = allocatedBy;
        AllocatedByDevice = allocatedByDevice;
    }

    public int Id { get; }
    public RelationshipTemplateId RelationshipTemplateId { get; set; }
    public IdentityAddress AllocatedBy { get; set; }
    public DateTime AllocatedAt { get; set; }
    public DeviceId AllocatedByDevice { get; set; }
}
