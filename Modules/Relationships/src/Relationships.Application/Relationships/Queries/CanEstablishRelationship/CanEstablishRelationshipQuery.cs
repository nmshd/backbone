using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.CanEstablishRelationship;

public class CanEstablishRelationshipQuery : IRequest<CanEstablishRelationshipResponse>
{
    public required string PeerAddress { get; init; }
}
