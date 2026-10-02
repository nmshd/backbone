using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Module.Features.Shared;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.AcceptRelationship;

public class AcceptRelationshipResponse : RelationshipMetadataDTO
{
    public AcceptRelationshipResponse(Relationship relationship) : base(relationship)
    {
    }
}
