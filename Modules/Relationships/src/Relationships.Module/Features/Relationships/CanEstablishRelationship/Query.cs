using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.CanEstablishRelationship;

public class CanEstablishRelationshipQuery : IRequest<CanEstablishRelationshipResponse>
{
    public required string PeerAddress { get; init; }
}
