using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Abstractions;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.GetPeerOfActiveIdentityInRelationship;

public class Handler : IRequestHandler<Query, Response>
{
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IRelationshipsRepository relationshipsRepository)
    {
        _relationshipsRepository = relationshipsRepository;
        _userContext = userContext;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var peerIdentityAddress = await _relationshipsRepository.GetRelationshipPeer(RelationshipId.Parse(request.Id), _userContext.GetAddress(), cancellationToken);
        return new Response { IdentityAddress = peerIdentityAddress };
    }
}
