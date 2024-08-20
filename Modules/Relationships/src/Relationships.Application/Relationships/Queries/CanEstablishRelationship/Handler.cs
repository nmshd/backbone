using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.CanEstablishRelationship;

public class Handler : IRequestHandler<CanEstablishRelationshipQuery, CanEstablishRelationshipResponse>
{
    private static readonly CanEstablishRelationshipResponse TRUE = new() { CanCreate = true };
    private static readonly CanEstablishRelationshipResponse FALSE = new() { CanCreate = false };
    private static readonly IList<RelationshipStatus> FORBIDDEN_STATUSES = [RelationshipStatus.Pending, RelationshipStatus.Active, RelationshipStatus.Terminated, RelationshipStatus.DeletionProposed];

    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IRelationshipsRepository relationshipsRepository)
    {
        _relationshipsRepository = relationshipsRepository;
        _userContext = userContext;
    }

    public async Task<CanEstablishRelationshipResponse> Handle(CanEstablishRelationshipQuery request, CancellationToken cancellationToken)
    {
        var identity = _userContext.GetAddress();
        var peer = request.PeerAddress;
        var relationships = await _relationshipsRepository.FindRelationships(BetweenParticipants(identity, peer), cancellationToken);

        return relationships.Any(relationship => FORBIDDEN_STATUSES.Contains(relationship.Status)) ? FALSE : TRUE;
    }

    private static Expression<Func<Relationship, bool>> BetweenParticipants(IdentityAddress a, IdentityAddress b)
    {
        return r => (r.From == a && r.To == b) || (r.From == b && r.To == a);
    }
}
