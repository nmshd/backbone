using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;

public class ListRelationshipTemplatesQuery : IRequest<ListRelationshipTemplatesResponse>
{
    public ListRelationshipTemplatesQuery(PaginationFilter paginationFilter, IEnumerable<RelationshipTemplateQuery>? queries)
    {
        PaginationFilter = paginationFilter;
        Queries = queries == null ? [] : queries.ToList();
    }

    public PaginationFilter PaginationFilter { get; set; }
    public List<RelationshipTemplateQuery> Queries { get; set; }
}

public class RelationshipTemplateQuery
{
    public required string Id { get; set; }
    public byte[]? Password { get; set; }
}
