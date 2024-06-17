using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.GetPeerOfActiveIdentityInRelationship;

public class Handler : IRequestHandler<GetPeerOfActiveIdentityInRelationshipQuery, GetPeerOfActiveIdentityInRelationshipResponse>
{
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IRelationshipsRepository relationshipsRepository)
    {
        _relationshipsRepository = relationshipsRepository;
        _userContext = userContext;
    }

    public async Task<GetPeerOfActiveIdentityInRelationshipResponse> Handle(GetPeerOfActiveIdentityInRelationshipQuery request, CancellationToken cancellationToken)
    {
        var peerIdentityAddress = await _relationshipsRepository.FindRelationshipPeer(request.Id, _userContext.GetAddress(), cancellationToken);
        return new GetPeerOfActiveIdentityInRelationshipResponse { IdentityAddress = peerIdentityAddress };
    }
}
