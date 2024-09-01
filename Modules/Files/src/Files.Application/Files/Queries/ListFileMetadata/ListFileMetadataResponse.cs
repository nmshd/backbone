using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Files.Application.Files.DTOs;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Application.Files.Queries.ListFileMetadata;

public class ListFileMetadataResponse : PagedResponse<FileMetadataDTO>
{
    public ListFileMetadataResponse(DbPaginationResult<File> dbPaginationResult, PaginationFilter previousFilter) : base(dbPaginationResult.ItemsOnPage.Select(f => new FileMetadataDTO(f)),
        previousFilter, dbPaginationResult.TotalNumberOfItems)
    {
    }
}
