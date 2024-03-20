using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipTerminationChange : RelationshipChange
{
    private RelationshipTerminationChange()
    {
    }

    internal RelationshipTerminationChange(Relationship relationship, IdentityAddress createdBy, DeviceId createdByDevice) : base(relationship, createdBy, createdByDevice,
        RelationshipChangeType.Termination, null)
    {
    }
}
