using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Enmeshed.BuildingBlocks.Application.Pagination;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationshipsByParticipantAddress;
public class ListRelationshipsByParticipantAddressResponse : PagedResponse<RelationshipByParticipantAddressDTO>
{
    public ListRelationshipsByParticipantAddressResponse(IEnumerable<RelationshipByParticipantAddressDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
