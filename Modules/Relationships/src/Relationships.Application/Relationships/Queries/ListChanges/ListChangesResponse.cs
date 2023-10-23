using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Relationships.Application.Relationships.DTOs;

namespace Backbone.Relationships.Application.Relationships.Queries.ListChanges;

public class ListChangesResponse : PagedResponse<RelationshipChangeDTO>
{
    public ListChangesResponse(IEnumerable<RelationshipChangeDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
