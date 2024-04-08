using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationships;

public class ListRelationshipsResponse : PagedResponse<RelationshipDTO>
{
    public ListRelationshipsResponse(IEnumerable<RelationshipDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
