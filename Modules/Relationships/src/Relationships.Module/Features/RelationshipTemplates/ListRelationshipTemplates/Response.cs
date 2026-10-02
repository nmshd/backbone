using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.Modules.Relationships.Module.Features.Shared;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.ListRelationshipTemplates;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListRelationshipTemplatesResponse")]
public class Response : PagedResponse<RelationshipTemplateDTO>
{
    public Response(DbPaginationResult<RelationshipTemplate> dbPaginationResult, PaginationFilter previousPaginationFilter) : base(
        dbPaginationResult.ItemsOnPage.Select(x => new RelationshipTemplateDTO(x)), previousPaginationFilter, dbPaginationResult.TotalNumberOfItems)
    {
    }
}
