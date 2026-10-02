using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Module.Features.Shared;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RejectRelationshipReactivation;

public class RejectRelationshipReactivationResponse : RelationshipMetadataDTO
{
    public RejectRelationshipReactivationResponse(Relationship relationship) : base(relationship)
    {
    }
}
