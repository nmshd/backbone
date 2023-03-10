using AutoMapper;
using Backbone.Modules.Relationships.Application.Extensions;
using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationships;

public class Handler : RequestHandlerBase<ListRelationshipsQuery, ListRelationshipsResponse>
{
    private readonly IContentStore _contentStore;

    public Handler(IRelationshipsDbContext dbContext, IUserContext userContext, IMapper mapper, IContentStore contentStore) : base(dbContext, userContext, mapper)
    {
        _contentStore = contentStore;
    }

    public override async Task<ListRelationshipsResponse> Handle(ListRelationshipsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext
            .Set<Relationship>()
            .IncludeAll()
            .AsNoTracking()
            .WithParticipant(_activeIdentity)
            .WithIdIn(request.Ids);

        var dbPaginationResult = await query.OrderAndPaginate(d => d.CreatedAt, request.PaginationFilter);

        var changes = dbPaginationResult.ItemsOnPage.SelectMany(r => r.Changes);

        await _contentStore.FillContentOfChanges(changes);

        return new ListRelationshipsResponse(_mapper.Map<RelationshipDTO[]>(dbPaginationResult.ItemsOnPage), request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
