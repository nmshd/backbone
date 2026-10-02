using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.GetPeerOfActiveIdentityInRelationship;

public class Query : IRequest<Response>
{
    public required string Id { get; init; }
}
