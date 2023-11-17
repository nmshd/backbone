using Backbone.Modules.Quotas.Application.DTOs;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.GetQuotasForIdentity;

public class ListQuotasForIdentityResponse
{
    public ListQuotasForIdentityResponse(IEnumerable<QuotaDTO> items)
    {
        Items = items;
    }

    public IEnumerable<QuotaDTO> Items { get; }
}
