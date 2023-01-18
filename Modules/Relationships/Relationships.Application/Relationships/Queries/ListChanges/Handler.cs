using AutoMapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Microsoft.EntityFrameworkCore;
using Relationships.Application.Extensions;
using Relationships.Application.Infrastructure;
using Relationships.Application.Relationships.DTOs;
using Relationships.Domain.Entities;

namespace Relationships.Application.Relationships.Queries.ListChanges;

public class Handler : RequestHandlerBase<ListChangesQuery, ListChangesResponse>
{
    private readonly IContentStore _contentStore;

    public Handler(IDbContext dbContext, IUserContext userContext, IMapper mapper, IContentStore contentStore) : base(dbContext, userContext, mapper)
    {
        _contentStore = contentStore;
    }

    public override async Task<ListChangesResponse> Handle(ListChangesQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext
            .Set<RelationshipChange>()
            .IncludeAll()
            .AsNoTracking()
            .WithType(request.Type)
            .WithStatus(request.Status)
            .ModifiedAt(request.ModifiedAt)
            .CreatedAt(request.CreatedAt)
            .CompletedAt(request.CompletedAt)
            .CreatedBy(request.CreatedBy)
            .CompletedBy(request.CompletedBy)
            .WithRelationshipParticipant(_activeIdentity);

        if (request.Ids.Any())
            query = query.WithIdIn(request.Ids);

        if (request.OnlyPeerChanges)
            query = query.OnlyPeerChanges(_activeIdentity);
        
        var dbPaginationResult = await query.OrderAndPaginate(d => d.CreatedAt, request.PaginationFilter);

        await _contentStore.FillContentOfChanges(dbPaginationResult.ItemsOnPage);

        return new ListChangesResponse(_mapper.Map<RelationshipChangeDTO[]>(dbPaginationResult.ItemsOnPage), request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
