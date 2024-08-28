using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;

public class ListRelationshipTemplatesResponse : PagedResponse<RelationshipTemplateDTO>
{
    public ListRelationshipTemplatesResponse(DbPaginationResult<RelationshipTemplate> dbPaginationResult, PaginationFilter previousPaginationFilter) : base(
        dbPaginationResult.ItemsOnPage.Select(x => new RelationshipTemplateDTO(x)), previousPaginationFilter, dbPaginationResult.TotalNumberOfItems)
    {
    }
}
