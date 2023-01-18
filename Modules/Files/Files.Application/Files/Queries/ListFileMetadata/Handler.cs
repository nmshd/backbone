using AutoMapper;
using AutoMapper.QueryableExtensions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Files.Application.Extensions;
using Files.Application.Files.DTOs;
using Files.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Files.Application.Files.Queries.ListFileMetadata;

public class Handler : RequestHandlerBase<ListFileMetadataQuery, ListFileMetadataResponse>
{
    public Handler(IDbContext dbContext, IUserContext userContext, IMapper mapper) : base(dbContext, userContext, mapper) { }

    public override async Task<ListFileMetadataResponse> Handle(ListFileMetadataQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext
            .SetReadOnly<FileMetadata>()
            .CreatedBy(_activeIdentity)
            .NotExpired()
            .NotDeleted();

        if (request.Ids.Any())
            query = query.WithIdIn(request.Ids);

        var dbPaginationResult = await query.OrderAndPaginate(d => d.CreatedAt, request.PaginationFilter);
        var items = _mapper.Map<FileMetadataDTO[]>(dbPaginationResult.ItemsOnPage);
        
        var response = new ListFileMetadataResponse(items, request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);

        return response;
    }
}
