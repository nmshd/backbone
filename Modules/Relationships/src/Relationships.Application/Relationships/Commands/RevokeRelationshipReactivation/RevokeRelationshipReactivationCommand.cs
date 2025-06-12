using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.RevokeRelationshipReactivation;

public class RevokeRelationshipReactivationCommand : IRequest<RevokeRelationshipReactivationResponse>
{
    public required string RelationshipId { get; init; }
}
