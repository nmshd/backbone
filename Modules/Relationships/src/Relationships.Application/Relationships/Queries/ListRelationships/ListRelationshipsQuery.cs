using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationships;

public class ListRelationshipsQuery : IRequest<ListRelationshipsResponse>
{
    public ListRelationshipsQuery(PaginationFilter paginationFilter, IEnumerable<string>? ids)
    {
        PaginationFilter = paginationFilter;
        Ids = ids == null ? [] : ids.ToList();
    }

    public PaginationFilter PaginationFilter { get; set; }
    public List<string> Ids { get; set; }
}
