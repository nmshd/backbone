using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Module.Features.Shared;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RevokeRelationship;

public class RevokeRelationshipResponse : RelationshipMetadataDTO
{
    public RevokeRelationshipResponse(Relationship relationship) : base(relationship)
    {
    }
}
