using Backbone.Modules.Files.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Files.Application.Files.Queries.ListFileMetadata;

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
