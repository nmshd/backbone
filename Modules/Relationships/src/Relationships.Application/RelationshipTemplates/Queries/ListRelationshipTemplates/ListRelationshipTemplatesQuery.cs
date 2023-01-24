using Enmeshed.BuildingBlocks.Application.Pagination;
using MediatR;
using Relationships.Domain.Ids;

namespace Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;

public class ListRelationshipTemplatesQuery : IRequest<ListRelationshipTemplatesResponse>
{
    public ListRelationshipTemplatesQuery(PaginationFilter paginationFilter, IEnumerable<RelationshipTemplateId> ids)
    {
        PaginationFilter = paginationFilter;
        Ids = ids == null ? null : new List<RelationshipTemplateId>(ids);
    }

    public PaginationFilter PaginationFilter { get; set; }
    public List<RelationshipTemplateId> Ids { get; set; }
}
