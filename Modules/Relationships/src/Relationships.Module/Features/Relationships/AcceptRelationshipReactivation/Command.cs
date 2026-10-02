using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.AcceptRelationshipReactivation;

public class AcceptRelationshipReactivationCommand : IRequest<AcceptRelationshipReactivationResponse>
{
    public required string RelationshipId { get; init; }
}
