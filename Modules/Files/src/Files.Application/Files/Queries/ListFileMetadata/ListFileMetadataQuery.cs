using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Files.Domain.Entities;
using MediatR;

namespace Backbone.Files.Application.Files.Queries.ListFileMetadata;

public class ListFileMetadataQuery : IRequest<ListFileMetadataResponse>
{
    public ListFileMetadataQuery(PaginationFilter paginationFilter, IEnumerable<FileId> ids)
    {
        PaginationFilter = paginationFilter;
        Ids = ids;
    }

    public PaginationFilter PaginationFilter { get; set; }

    public IEnumerable<FileId> Ids { get; set; }
}
