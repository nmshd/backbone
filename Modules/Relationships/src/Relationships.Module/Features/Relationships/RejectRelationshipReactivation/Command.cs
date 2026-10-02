using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RejectRelationshipReactivation;

public class Command : IRequest<Response>
{
    public required string RelationshipId { get; init; }
}
