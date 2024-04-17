using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.TerminateRelationship;
public class TerminateRelationshipResponse : RelationshipMetadataDTO
{
    public TerminateRelationshipResponse(Relationship relationship) : base(relationship) { }
}
