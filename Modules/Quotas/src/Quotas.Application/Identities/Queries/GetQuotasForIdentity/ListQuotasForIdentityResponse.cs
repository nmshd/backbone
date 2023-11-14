using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Quotas.Application.DTOs;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.GetQuotasForIdentity;

public class ListQuotasForIdentityResponse
{
    public IEnumerable<QuotaDTO> Items { get; }

    public ListQuotasForIdentityResponse(IEnumerable<QuotaDTO> items)
    {
        Items = items;
    }
}
