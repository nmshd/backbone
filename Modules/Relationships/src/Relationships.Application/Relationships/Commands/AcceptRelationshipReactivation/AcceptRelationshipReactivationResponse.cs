using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.AcceptRelationshipReactivation;
public class AcceptRelationshipReactivationResponse : RelationshipMetadataDTO
{
    public AcceptRelationshipReactivationResponse(Relationship relationship) : base(relationship)
    {
    }
}
