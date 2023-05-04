using AutoMapper;
using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationships;

public class Handler : RequestHandlerBase<ListRelationshipsQuery, ListRelationshipsResponse>
{
    private readonly IRelationshipsRepository _relationshipsRepository;

    public Handler(IRelationshipsDbContext dbContext, IUserContext userContext, IMapper mapper, IRelationshipsRepository relationshipsRepository) : base(dbContext, userContext, mapper)
    {
        _relationshipsRepository = relationshipsRepository;
    }

    public override async Task<ListRelationshipsResponse> Handle(ListRelationshipsQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await _relationshipsRepository.FindRelationshipsWithIds(request.Ids, _activeIdentity, request.PaginationFilter, track: false);

        return new ListRelationshipsResponse(_mapper.Map<RelationshipDTO[]>(dbPaginationResult.ItemsOnPage), request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
