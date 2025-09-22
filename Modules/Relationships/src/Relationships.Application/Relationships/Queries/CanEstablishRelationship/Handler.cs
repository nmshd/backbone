using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.CanEstablishRelationship;

public class Handler : IRequestHandler<CanEstablishRelationshipQuery, CanEstablishRelationshipResponse>
{
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IRelationshipsRepository relationshipsRepository)
    {
        _relationshipsRepository = relationshipsRepository;
        _userContext = userContext;
    }

    public async Task<CanEstablishRelationshipResponse> Handle(CanEstablishRelationshipQuery request, CancellationToken cancellationToken)
    {
        var existingRelationships = await _relationshipsRepository.ListWithoutContent(
            Relationship.IsBetween(request.PeerAddress, _userContext.GetAddress()),
            cancellationToken
        );

        var error = Relationship.CanEstablish(existingRelationships.ToList());

        return new CanEstablishRelationshipResponse { CanCreate = error == null, Code = error?.Code };
    }
}
