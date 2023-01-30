using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Enmeshed.BuildingBlocks.Application.Pagination;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListChanges;

public class ListChangesResponse : PagedResponse<RelationshipChangeDTO>
{
    public ListChangesResponse(IEnumerable<RelationshipChangeDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
