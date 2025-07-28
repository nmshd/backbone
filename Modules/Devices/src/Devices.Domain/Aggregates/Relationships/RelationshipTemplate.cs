using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Domain.Aggregates.Relationships;

public class RelationshipTemplate : Entity
{
    protected RelationshipTemplate()
    {
    }

    public string Id { get; } = null!;
    public IdentityAddress CreatedBy { get; } = null!;
    public virtual List<RelationshipTemplateAllocation> Allocations { get; } = null!;
}
