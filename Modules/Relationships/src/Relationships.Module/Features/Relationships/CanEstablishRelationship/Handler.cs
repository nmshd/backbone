using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Abstractions;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.CanEstablishRelationship;

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
        var existingRelationships = await _relationshipsRepository.ListWithoutContent(
            Relationship.IsBetween(request.PeerAddress, _userContext.GetAddress()),
            cancellationToken
        );

        var error = Relationship.CanEstablish(existingRelationships.ToList());

        return new Response { CanCreate = error == null, Code = error?.Code };
    }
}
