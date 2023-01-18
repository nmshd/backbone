using Enmeshed.BuildingBlocks.Application.Pagination;
using Relationships.Application.Relationships.DTOs;

namespace Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;

public class ListRelationshipTemplatesResponse : PagedResponse<RelationshipTemplateDTO>
{
    public ListRelationshipTemplatesResponse(IEnumerable<RelationshipTemplateDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
