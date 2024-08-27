using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationships;

public class ListRelationshipsResponse : PagedResponse<RelationshipDTO>
{
    public ListRelationshipsResponse(DbPaginationResult<Relationship> dbPaginationResult, PaginationFilter previousPaginationFilter) : base(
        dbPaginationResult.ItemsOnPage.Select(r => new RelationshipDTO(r)), previousPaginationFilter, dbPaginationResult.TotalNumberOfItems)
    {
    }
}
