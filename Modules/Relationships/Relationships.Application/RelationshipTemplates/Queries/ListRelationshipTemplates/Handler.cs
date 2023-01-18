using AutoMapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Microsoft.EntityFrameworkCore;
using Relationships.Application.Extensions;
using Relationships.Application.Infrastructure;
using Relationships.Application.Relationships;
using Relationships.Application.Relationships.DTOs;
using Relationships.Domain.Entities;

namespace Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;

public class Handler : RequestHandlerBase<ListRelationshipTemplatesQuery, ListRelationshipTemplatesResponse>
{
    private readonly IContentStore _contentStore;

    public Handler(IDbContext dbContext, IUserContext userContext, IMapper mapper, IContentStore contentStore) : base(dbContext, userContext, mapper)
    {
        _contentStore = contentStore;
    }

    public override async Task<ListRelationshipTemplatesResponse> Handle(ListRelationshipTemplatesQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext
            .SetReadOnly<RelationshipTemplate>()
            .NotExpiredFor(_activeIdentity)
            .NotDeleted()
            .WithIdIn(request.Ids);

        var dbPaginationResult = await query.OrderAndPaginate(d => d.CreatedAt, request.PaginationFilter);

        await _contentStore.FillContentOfTemplates(dbPaginationResult.ItemsOnPage);

        return new ListRelationshipTemplatesResponse(_mapper.Map<RelationshipTemplateDTO[]>(dbPaginationResult.ItemsOnPage), request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }
}
