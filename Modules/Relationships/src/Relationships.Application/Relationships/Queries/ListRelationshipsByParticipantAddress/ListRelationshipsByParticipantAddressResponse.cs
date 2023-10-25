using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Enmeshed.BuildingBlocks.Application.Pagination;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationshipsByParticipantAddress;
public class ListRelationshipsByParticipantAddressResponse : PagedResponse<RelationshipDTO>
{
    public ListRelationshipsByParticipantAddressResponse(IEnumerable<RelationshipDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
