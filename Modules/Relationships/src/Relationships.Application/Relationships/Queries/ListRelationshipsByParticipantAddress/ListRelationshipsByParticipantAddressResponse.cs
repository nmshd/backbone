using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationshipsByParticipantAddress;
public class ListRelationshipsByParticipantAddressResponse : PagedResponse<RelationshipByParticipantAddressDTO>
{
    public ListRelationshipsByParticipantAddressResponse(IEnumerable<RelationshipByParticipantAddressDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
