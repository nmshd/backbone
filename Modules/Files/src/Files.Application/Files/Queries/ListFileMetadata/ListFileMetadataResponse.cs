using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Files.Application.Files.DTOs;

namespace Backbone.Files.Application.Files.Queries.ListFileMetadata;

public class ListFileMetadataResponse : PagedResponse<FileMetadataDTO>
{
    public ListFileMetadataResponse(IEnumerable<FileMetadataDTO> items, PaginationFilter previousFilter, int totalRecords) : base(items, previousFilter, totalRecords) { }
}
