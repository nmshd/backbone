using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RelationshipReactivationRequest;
public class RelationshipReactivationRequestCommand : IRequest<RelationshipReactivationRequestResponse>
{
    public required string RelationshipId { get; set; }
}
