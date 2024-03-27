using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RejectRelationship;

public class RejectRelationshipResponse : RelationshipMetadataDTO
{
    public RejectRelationshipResponse(Relationship relationship) : base(relationship)
    {
    }
}
