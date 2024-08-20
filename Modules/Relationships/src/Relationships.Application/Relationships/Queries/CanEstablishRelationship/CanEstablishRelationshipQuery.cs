using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.CanEstablishRelationship;

public class CanEstablishRelationshipQuery : IRequest<CanEstablishRelationshipResponse>
{
    public required IdentityAddress PeerAddress { get; set; }
}
