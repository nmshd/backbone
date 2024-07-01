using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RevokeRelationship;

public class RevokeRelationshipResponse : RelationshipMetadataDTO
{
    public RevokeRelationshipResponse(Relationship relationship) : base(relationship)
    {
    }
}
