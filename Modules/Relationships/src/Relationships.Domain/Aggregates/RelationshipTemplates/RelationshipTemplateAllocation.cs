using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;

namespace Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

public class RelationshipTemplateAllocation : Entity
{
    // ReSharper disable once UnusedMember.Local
    private RelationshipTemplateAllocation()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        RelationshipTemplateId = null!;
        AllocatedBy = null!;
        AllocatedByDevice = null!;
    }

    public RelationshipTemplateAllocation(RelationshipTemplateId relationshipTemplateId, IdentityAddress allocatedBy, DeviceId allocatedByDevice)
    {
        RelationshipTemplateId = relationshipTemplateId;
        AllocatedAt = SystemTime.UtcNow;
        AllocatedBy = allocatedBy;
        AllocatedByDevice = allocatedByDevice;
    }

    public int Id { get; }
    public RelationshipTemplateId RelationshipTemplateId { get; set; }
    public RelationshipTemplate RelationshipTemplate { get; } = null!;
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
