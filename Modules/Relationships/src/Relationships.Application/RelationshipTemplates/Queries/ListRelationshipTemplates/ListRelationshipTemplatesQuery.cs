using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;

public class ListRelationshipTemplatesQuery : IRequest<ListRelationshipTemplatesResponse>
{
    public required PaginationFilter PaginationFilter { get; init; }
    public required List<ListRelationshipTemplatesQueryItem> QueryItems { get; init; }
}

public class ListRelationshipTemplatesQueryItem
{
    public required string Id { get; set; }
    public byte[]? Password { get; set; }
}
