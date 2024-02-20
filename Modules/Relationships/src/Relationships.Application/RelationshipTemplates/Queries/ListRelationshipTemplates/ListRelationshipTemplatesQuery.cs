using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Relationships.Domain.Ids;
using MediatR;

namespace Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;

public class ListRelationshipTemplatesQuery : IRequest<ListRelationshipTemplatesResponse>
{
    public ListRelationshipTemplatesQuery(PaginationFilter paginationFilter, IEnumerable<RelationshipTemplateId>? ids)
    {
        PaginationFilter = paginationFilter;
        Ids = ids == null ? [] : [..ids];
    }

    public PaginationFilter PaginationFilter { get; set; }
    public List<RelationshipTemplateId> Ids { get; set; }
}
