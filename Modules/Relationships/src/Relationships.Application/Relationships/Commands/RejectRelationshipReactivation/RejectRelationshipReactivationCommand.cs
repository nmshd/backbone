using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RejectRelationshipReactivation;

public class RejectRelationshipReactivationCommand : IRequest<RejectRelationshipReactivationResponse>
{
    public required string RelationshipId { get; init; }
}
