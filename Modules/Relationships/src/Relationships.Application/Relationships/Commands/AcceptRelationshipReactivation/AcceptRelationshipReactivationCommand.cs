using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.AcceptRelationshipReactivation;

public class AcceptRelationshipReactivationCommand : IRequest<AcceptRelationshipReactivationResponse>
{
    public required string RelationshipId { get; init; }
}
