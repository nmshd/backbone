using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationships;

public class Handler : IRequestHandler<ListRelationshipsQuery, ListRelationshipsResponse>
{
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IRelationshipsRepository relationshipsRepository)
    {
        _relationshipsRepository = relationshipsRepository;
        _userContext = userContext;
    }

    public async Task<ListRelationshipsResponse> Handle(ListRelationshipsQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult =
            await _relationshipsRepository.ListRelationshipsWithContent(request.Ids.Select(RelationshipId.Parse), _userContext.GetAddress(), request.PaginationFilter, cancellationToken, track: false);

        return new ListRelationshipsResponse(dbPaginationResult, request.PaginationFilter);
    }
}
