using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.ReactivateRelationship;
public class ReactivateRelationshipResponse : RelationshipMetadataDTO
{
    public ReactivateRelationshipResponse(Relationship relationship) : base(relationship)
    {
    }
}
