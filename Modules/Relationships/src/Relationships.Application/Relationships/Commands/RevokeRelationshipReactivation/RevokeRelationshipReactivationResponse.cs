using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RevokeRelationshipReactivation;
public class RevokeRelationshipReactivationResponse : RelationshipMetadataDTO
{
    public RevokeRelationshipReactivationResponse(Relationship relationship) : base(relationship) { }
}
