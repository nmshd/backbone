using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Enmeshed.BuildingBlocks.Application.Pagination;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationships;

public class ListRelationshipsResponse : PagedResponse<RelationshipDTO>
{
    public ListRelationshipsResponse(IEnumerable<RelationshipDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
