using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RevokeRelationshipReactivation;

public class RevokeRelationshipReactivationCommand : IRequest<RevokeRelationshipReactivationResponse>
{
    public required string RelationshipId { get; init; }
}
