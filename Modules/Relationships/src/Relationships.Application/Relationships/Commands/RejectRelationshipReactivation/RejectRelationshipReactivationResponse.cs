using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RejectRelationshipReactivation;
public class RejectRelationshipReactivationResponse : RelationshipMetadataDTO
{
    public RejectRelationshipReactivationResponse(Relationship relationship) : base(relationship)
    {
    }
}
