using Backbone.Modules.Relationships.Domain.Ids;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;

namespace Backbone.Modules.Relationships.Domain.Entities;

public class RelationshipTemplateAllocation
{
    public RelationshipTemplateAllocation(RelationshipTemplateId relationshipTemplateId, IdentityAddress allocatedBy, DeviceId allocatedByDevice)
    {
        RelationshipTemplateId = relationshipTemplateId;
        AllocatedAt = SystemTime.UtcNow;
        AllocatedBy = allocatedBy;
        AllocatedByDevice = allocatedByDevice;
    }

    public RelationshipTemplateId RelationshipTemplateId { get; set; }
    public IdentityAddress AllocatedBy { get; set; }
    public DateTime AllocatedAt { get; set; }
    public DeviceId AllocatedByDevice { get; set; }
}
