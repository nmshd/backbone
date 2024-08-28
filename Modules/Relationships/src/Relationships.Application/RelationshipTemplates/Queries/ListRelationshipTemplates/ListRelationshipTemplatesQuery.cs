using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;

public class ListRelationshipTemplatesQuery : IRequest<ListRelationshipTemplatesResponse>
{
    public ListRelationshipTemplatesQuery(PaginationFilter paginationFilter, IEnumerable<string>? ids)
    {
        PaginationFilter = paginationFilter;
        Ids = ids == null ? [] : ids.ToList();
    }

    public PaginationFilter PaginationFilter { get; set; }
    public List<string> Ids { get; set; }
}
