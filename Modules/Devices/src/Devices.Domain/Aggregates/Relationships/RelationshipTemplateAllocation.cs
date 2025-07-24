using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Domain.Aggregates.Relationships;

public class RelationshipTemplateAllocation : Entity
{
    private RelationshipTemplateAllocation()
    {
    }

    public string Id { get; } = null!;
    public virtual RelationshipTemplate RelationshipTemplate { get; } = null!;
    public IdentityAddress AllocatedBy { get; } = null!;

    public static Expression<Func<RelationshipTemplateAllocation, bool>> IsAllocatedBy(IdentityAddress activeIdentity)
    {
        return a => a.AllocatedBy == activeIdentity;
    }

    public static Expression<Func<RelationshipTemplateAllocation, bool>> BelongsToTemplateOf(IdentityAddress peerAddress)
    {
        return a => a.RelationshipTemplate.CreatedBy == peerAddress;
    }
}
