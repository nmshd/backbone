using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListChanges;

public class ListChangesResponse : PagedResponse<RelationshipChangeDTO>
{
    public ListChangesResponse(IEnumerable<RelationshipChangeDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
