using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Abstractions;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.ListRelationships;

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
        var dbPaginationResult =
            await _relationshipsRepository.ListRelationshipsWithContent(request.Ids.Select(RelationshipId.Parse), _userContext.GetAddress(), request.PaginationFilter, cancellationToken, track: false);

        return new Response(dbPaginationResult, request.PaginationFilter);
    }
}
