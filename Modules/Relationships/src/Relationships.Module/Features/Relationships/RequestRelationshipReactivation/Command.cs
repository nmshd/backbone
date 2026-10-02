using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RequestRelationshipReactivation;

public class RequestRelationshipReactivationCommand : IRequest<RequestRelationshipReactivationResponse>
{
    public required string RelationshipId { get; init; }
}
