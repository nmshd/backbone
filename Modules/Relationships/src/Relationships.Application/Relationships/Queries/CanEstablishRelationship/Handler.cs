using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.CanEstablishRelationship;

public class Handler : IRequestHandler<CanEstablishRelationshipQuery, CanEstablishRelationshipResponse>
{
    private static readonly CanEstablishRelationshipResponse TRUE = new() { CanCreate = true };
    private static readonly CanEstablishRelationshipResponse FALSE = new() { CanCreate = false };

    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IRelationshipsRepository relationshipsRepository)
    {
        _relationshipsRepository = relationshipsRepository;
        _userContext = userContext;
    }

    public async Task<CanEstablishRelationshipResponse> Handle(CanEstablishRelationshipQuery request, CancellationToken cancellationToken)
    {
        var hasActiveRelationship = await _relationshipsRepository.RelationshipBetweenTwoIdentitiesExists(_userContext.GetAddress(), IdentityAddress.Parse(request.PeerAddress), cancellationToken);

        return hasActiveRelationship ? FALSE : TRUE;
    }
}
