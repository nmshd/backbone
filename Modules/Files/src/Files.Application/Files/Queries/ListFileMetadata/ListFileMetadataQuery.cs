using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Files.Application.Files.Queries.ListFileMetadata;

public class ListFileMetadataQuery : IRequest<ListFileMetadataResponse>
{
    public ListFileMetadataQuery(PaginationFilter paginationFilter, IEnumerable<string> ids)
    {
        PaginationFilter = paginationFilter;
        Ids = ids;
    }

    public PaginationFilter PaginationFilter { get; set; }

    public IEnumerable<string> Ids { get; set; }
}
