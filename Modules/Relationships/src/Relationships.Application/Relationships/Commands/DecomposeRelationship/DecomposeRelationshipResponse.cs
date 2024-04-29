using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.DecomposeRelationship;
public class DecomposeRelationshipResponse : RelationshipMetadataDTO
{
    public DecomposeRelationshipResponse(Relationship relationship) : base(relationship)
    {
    }
}
