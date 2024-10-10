using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;

public class ListRelationshipTemplatesQuery : IRequest<ListRelationshipTemplatesResponse>
{
    public ListRelationshipTemplatesQuery(PaginationFilter paginationFilter, IEnumerable<RelationshipTemplateQueryItem>? queries)
    {
        PaginationFilter = paginationFilter;
        QueryItems = queries == null ? [] : queries.ToList();
    }

    public PaginationFilter PaginationFilter { get; set; }
    public List<RelationshipTemplateQueryItem> QueryItems { get; set; }
}

public class RelationshipTemplateQueryItem
{
    public required string Id { get; set; }
    public byte[]? Password { get; set; }
}
