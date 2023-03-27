using Backbone.Modules.Devices.Application.DTOs;
using Enmeshed.BuildingBlocks.Application.Pagination;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;
public class ListIdentitiesResponse : PagedResponse<IdentityDTO>
{
    public ListIdentitiesResponse(IEnumerable<IdentityDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords)
    {
        Identities = items.ToList();
    }

    public List<IdentityDTO> Identities { get; set; }
}
