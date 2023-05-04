using AutoMapper;
using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListChanges;

public class Handler : RequestHandlerBase<ListChangesQuery, ListChangesResponse>
{
    private readonly IRelationshipsRepository _relationshipsRepository;

    public Handler(IRelationshipsDbContext dbContext, IUserContext userContext, IMapper mapper, IRelationshipsRepository relationshipsRepository) : base(dbContext, userContext, mapper)
    {
        _relationshipsRepository = relationshipsRepository;
    }

    public override async Task<ListChangesResponse> Handle(ListChangesQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await _relationshipsRepository.FindChangesWithIds(request.Ids, request.Type, request.Status, request.ModifiedAt, request.CreatedAt, request.CompletedAt, request.CreatedBy, request.CompletedBy, _activeIdentity, request.PaginationFilter, track: false);

        return new ListChangesResponse(_mapper.Map<RelationshipChangeDTO[]>(dbPaginationResult.ItemsOnPage), request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
