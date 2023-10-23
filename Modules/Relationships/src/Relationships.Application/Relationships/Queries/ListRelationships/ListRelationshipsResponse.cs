using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Relationships.Application.Relationships.DTOs;

namespace Backbone.Relationships.Application.Relationships.Queries.ListRelationships;

public class ListRelationshipsResponse : PagedResponse<RelationshipDTO>
{
    public ListRelationshipsResponse(IEnumerable<RelationshipDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
