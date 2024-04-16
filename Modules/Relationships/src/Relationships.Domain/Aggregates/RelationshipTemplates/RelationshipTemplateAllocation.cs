using System.Linq.Expressions;
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
    public IdentityAddress AllocatedBy { get; private set; }
    public DateTime AllocatedAt { get; set; }
    public DeviceId AllocatedByDevice { get; set; }

    public bool ReplaceIdentityAddress(IdentityAddress oldIdentityAddress, IdentityAddress newIdentityAddress)
    {
        if (AllocatedBy != oldIdentityAddress) return false;

        AllocatedBy = newIdentityAddress;
        return true;
    }

    public static Expression<Func<RelationshipTemplateAllocation, bool>> WasAllocatedBy(IdentityAddress allocatedBy)
    {
        return x => x.AllocatedBy == allocatedBy.ToString();
    }
}
