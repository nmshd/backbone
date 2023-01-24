using Enmeshed.BuildingBlocks.Application.Pagination;
using Relationships.Application.Relationships.DTOs;

namespace Relationships.Application.Relationships.Queries.ListRelationships;

public class ListRelationshipsResponse : PagedResponse<RelationshipDTO>
{
    public ListRelationshipsResponse(IEnumerable<RelationshipDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
