using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RequestRelationshipReactivation;

public class RequestRelationshipReactivationCommand : IRequest<RequestRelationshipReactivationResponse>
{
    public required string RelationshipId { get; init; }
}
