using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.TerminateRelationship;
public class TerminateRelationshipCommand : IRequest<TerminateRelationshipResponse>
{
    public required string RelationshipId { get; set; }
}
