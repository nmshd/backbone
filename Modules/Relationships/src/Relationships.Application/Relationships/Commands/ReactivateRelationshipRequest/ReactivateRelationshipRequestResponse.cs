using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.ReactivateRelationshipRequest;
public class ReactivateRelationshipRequestResponse : RelationshipMetadataDTO
{
    public ReactivateRelationshipRequestResponse(Relationship relationship) : base(relationship)
    {
    }
}
