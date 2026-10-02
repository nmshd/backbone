using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Module.Features.Shared;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.AcceptRelationshipReactivation;

public class AcceptRelationshipReactivationResponse : RelationshipMetadataDTO
{
    public AcceptRelationshipReactivationResponse(Relationship relationship) : base(relationship)
    {
    }
}
