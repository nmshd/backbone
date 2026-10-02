using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Module.Features.Shared;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RejectRelationship;

public class RejectRelationshipResponse : RelationshipMetadataDTO
{
    public RejectRelationshipResponse(Relationship relationship) : base(relationship)
    {
    }
}
