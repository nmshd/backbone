using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RelationshipReactivationRequest;
public class RequestRelationshipReactivationCommand : IRequest<RelationshipReactivationRequestResponse>
{
    public required string RelationshipId { get; set; }
}
