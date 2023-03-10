using AutoMapper;
using Backbone.Modules.Files.Application.Extensions;
using Backbone.Modules.Files.Application.Files.DTOs;
using Backbone.Modules.Files.Application.Infrastructure.Persistence;
using Backbone.Modules.Files.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.BuildingBlocks.Application.Extensions;

namespace Backbone.Modules.Files.Application.Files.Queries.ListFileMetadata;

public class Handler : RequestHandlerBase<ListFileMetadataQuery, ListFileMetadataResponse>
{
    public Handler(IFilesDbContext dbContext, IUserContext userContext, IMapper mapper) : base(dbContext, userContext, mapper) { }

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
