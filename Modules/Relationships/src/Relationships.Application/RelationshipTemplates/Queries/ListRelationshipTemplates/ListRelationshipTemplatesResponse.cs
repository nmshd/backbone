using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Relationships.Application.Relationships.DTOs;

namespace Backbone.Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;

public class ListRelationshipTemplatesResponse : PagedResponse<RelationshipTemplateDTO>
{
    public ListRelationshipTemplatesResponse(IEnumerable<RelationshipTemplateDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
