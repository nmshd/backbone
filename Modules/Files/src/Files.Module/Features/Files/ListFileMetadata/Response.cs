using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Files.Module.Features.Files.Shared;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Backbone.Modules.Files.Module.Features.Files.ListFileMetadata;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListFileMetadataResponse")]
public class Response : PagedResponse<FileMetadataDTO>
{
    public Response(DbPaginationResult<File> dbPaginationResult, PaginationFilter previousFilter) : base(dbPaginationResult.ItemsOnPage.Select(f => new FileMetadataDTO(f)),
        previousFilter, dbPaginationResult.TotalNumberOfItems)
    {
    }
}
