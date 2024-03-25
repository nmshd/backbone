using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.AcceptRelationship;

public class AcceptRelationshipResponse : RelationshipMetadataDTO
{
    public AcceptRelationshipResponse(Relationship relationship) : base(relationship)
    {
    }
}
