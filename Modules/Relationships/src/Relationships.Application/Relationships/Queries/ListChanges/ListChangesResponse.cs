using Enmeshed.BuildingBlocks.Application.Pagination;
using Relationships.Application.Relationships.DTOs;

namespace Relationships.Application.Relationships.Queries.ListChanges;

public class ListChangesResponse : PagedResponse<RelationshipChangeDTO>
{
    public ListChangesResponse(IEnumerable<RelationshipChangeDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
