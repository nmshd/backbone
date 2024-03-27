using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.CreateRelationship;

public class CreateRelationshipResponse : RelationshipMetadataDTO
{
    public CreateRelationshipResponse(Relationship relationship) : base(relationship)
    {
    }
}
