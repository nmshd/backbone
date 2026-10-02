using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RevokeRelationshipReactivation;

public class Command : IRequest<Response>
{
    public required string RelationshipId { get; init; }
}
