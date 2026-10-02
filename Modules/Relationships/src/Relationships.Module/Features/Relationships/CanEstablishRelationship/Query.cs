using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.CanEstablishRelationship;

public class Query : IRequest<Response>
{
    public required string PeerAddress { get; init; }
}
