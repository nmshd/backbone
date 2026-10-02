using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RequestRelationshipReactivation;

public class Command : IRequest<Response>
{
    public required string RelationshipId { get; init; }
}
