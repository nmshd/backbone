using Enmeshed.BuildingBlocks.Application.Pagination;
using Files.Application.Files.DTOs;

namespace Files.Application.Files.Queries.ListFileMetadata;

public class ListFileMetadataResponse : PagedResponse<FileMetadataDTO>
{
    public ListFileMetadataResponse(IEnumerable<FileMetadataDTO> items, PaginationFilter previousFilter, int totalRecords) : base(items, previousFilter, totalRecords) { }
}
