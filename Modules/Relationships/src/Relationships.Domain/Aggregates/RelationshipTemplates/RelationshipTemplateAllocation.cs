using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;

namespace Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

public class RelationshipTemplateAllocation : Entity
{
    [UsedImplicitly(Reason = "This constructor is for EF Core only")]
    protected RelationshipTemplateAllocation()
    {
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
    public virtual RelationshipTemplate RelationshipTemplate { get; } = null!;
    public IdentityAddress AllocatedBy { get; private set; }
    public DateTime AllocatedAt { get; set; }
    public DeviceId AllocatedByDevice { get; set; }

    public static Expression<Func<RelationshipTemplateAllocation, bool>> WasAllocatedBy(IdentityAddress allocatedBy)
    {
        return x => x.AllocatedBy == allocatedBy;
    }

    public static Expression<Func<RelationshipTemplateAllocation, bool>> BelongsToTemplateCreatedBy(string identity)
    {
        return a => a.RelationshipTemplate.CreatedBy == identity;
    }

    public void Anonymize(string didDomainName)
    {
        AllocatedBy = IdentityAddress.GetAnonymized(didDomainName);
    }
}
