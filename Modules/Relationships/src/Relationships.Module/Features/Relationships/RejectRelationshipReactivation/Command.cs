using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RejectRelationshipReactivation;

public class RejectRelationshipReactivationCommand : IRequest<RejectRelationshipReactivationResponse>
{
    public required string RelationshipId { get; init; }
}
