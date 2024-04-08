using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;

public class ListRelationshipTemplatesResponse : PagedResponse<RelationshipTemplateDTO>
{
    public ListRelationshipTemplatesResponse(IEnumerable<RelationshipTemplateDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
