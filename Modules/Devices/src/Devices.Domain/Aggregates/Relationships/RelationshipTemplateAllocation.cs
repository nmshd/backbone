using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Domain.Aggregates.Relationships;

public class RelationshipTemplateAllocation : Entity
{
    private RelationshipTemplateAllocation()
    {
    }

    public string Id { get; } = null!;
    public RelationshipTemplate RelationshipTemplate { get; } = null!;
    public IdentityAddress AllocatedBy { get; } = null!;
}
