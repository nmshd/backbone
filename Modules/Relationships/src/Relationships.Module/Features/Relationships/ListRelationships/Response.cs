using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Module.Features.Shared;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.ListRelationships;

public class Response : PagedResponse<RelationshipDTO>
{
    public Response(DbPaginationResult<Relationship> dbPaginationResult, PaginationFilter previousPaginationFilter) : base(
        dbPaginationResult.ItemsOnPage.Select(r => new RelationshipDTO(r)), previousPaginationFilter, dbPaginationResult.TotalNumberOfItems)
    {
    }
}
